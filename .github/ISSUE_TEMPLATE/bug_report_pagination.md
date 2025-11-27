---
name: Bug Report - Pagination State Issue
about: Report the static PAGE_SIZE field and -1 sentinel value bug in BaseParams
title: '[BUG] Static PAGE_SIZE field causes shared state mutation and database errors'
labels: bug, high-priority, data-access
assignees: ''
---

## Bug Description
The `BaseParams` class uses a **static field** `PAGE_SIZE` which causes shared state mutation across all instances. Additionally, when `DoApplyPaging = false`, the code sets `parameters.Size = -1` and `parameters.Page = -1`, which causes database query errors.

## Affected Files
- `FitBridge_Application/Specifications/BaseParams.cs`
- `FitBridge_Application/Specifications/Gym/GetAllGymOwnerCustomer/GetAllGymOwnerCustomerSpec.cs`
- Any other specification classes that set Size/Page to -1 when paging is disabled

## Root Causes

### 1. Static PAGE_SIZE Field
```csharp
public static int PAGE_SIZE = 10;

public int Size
{
    get { return PAGE_SIZE; }
    set { PAGE_SIZE = value > _maxPageSize ? _maxPageSize : value; }
}
```

**Problem**: Because `PAGE_SIZE` is static, setting `Size` on **any** `BaseParams` instance mutates the value for **all** instances globally. This creates unpredictable behavior in concurrent requests.

### 2. Sentinel Value -1 Causes Database Errors
```csharp
if (parameters.DoApplyPaging)
{
    AddPaging((parameters.Page - 1) * parameters.Size, parameters.Size);
}
else
{
    parameters.Size = -1;
    parameters.Page = -1;
}
```

**Problem**: When `DoApplyPaging = false`, the code sets `Size = -1` and `Page = -1`. If these values are later used in database queries (e.g., LINQ `.Skip()` or `.Take()`), it causes errors or unexpected behavior.

## Steps to Reproduce
1. Create two concurrent requests with different page sizes
2. Set `DoApplyPaging = false` on one request
3. Observe that:
   - The second request's page size affects the first request
   - Database queries may fail with negative skip/take values
   - Pagination state becomes unpredictable

## Expected Behavior
- Each `BaseParams` instance should have independent `Size` and `Page` values
- When `DoApplyPaging = false`, the pagination should be cleanly disabled without sentinel values
- No database errors should occur regardless of paging state

## Proposed Solution

### Option 1: Make PAGE_SIZE Instance-Level (Recommended)
```csharp
public abstract class BaseParams
{
    private const int _maxPageSize = 20;
    private const int _defaultPageSize = 10;
    private int _pageSize = _defaultPageSize;

    public bool DoApplyPaging { get; set; } = true;
    public int Page { get; set; } = 1;
    
    public int Size
    {
        get => _pageSize;
        set => _pageSize = value > _maxPageSize ? _maxPageSize : value;
    }

    public string SortBy { get; set; } = "Id";
    public string SortOrder { get; set; } = "asc";
    public string SearchTerm { get; set; } = string.Empty;
}
```

### Option 2: Use Nullable Int (Alternative)
```csharp
public int? Page { get; set; } = 1;
public int? Size { get; set; } = 10;
```
Then check for `null` instead of `-1` to detect disabled paging.

### Option 3: Remove Mutation in Specification Classes
Don't mutate `parameters.Size` and `parameters.Page` to `-1`. Instead, rely solely on the `DoApplyPaging` flag to determine if paging was applied.

## Impact
- **Severity**: High
- **Scope**: All API endpoints using pagination
- **Risk**: Data inconsistency, database errors, unpredictable pagination behavior in production

## Additional Context
- Target Framework: .NET 9
- Language: C# 13.0
- Pattern: Specification pattern with Entity Framework Core

## Related Files to Review
- All classes inheriting from `BaseParams`
- All classes using `AddPaging()` method
- Database query execution logic

---

**Priority**: High - This affects core pagination functionality across the application.
