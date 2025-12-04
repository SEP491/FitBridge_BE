# ?? TEST CASES - LU?NG PHÂN PH?I TI?N H? TH?NG FITBRIDGE

## ?? T?NG QUAN

Document này t?ng h?p **toàn b? test cases** ?? ki?m tra dòng ti?n trong h? th?ng FitBridge, bao g?m:
- Mua Gym Course
- Mua Freelance PT Package  
- Extend Gym Course
- Extend Freelance PT Package
- Phân ph?i l?i nhu?n (Profit Distribution)

---

## ?? LU?NG TI?N T?NG QUÁT

### Công Th?c Tính Toán

#### 1. System Commission (Hoa h?ng h? th?ng)
```
System Profit = SubTotalPrice × CommissionRate

N?u có Coupon System:
    System Profit = (SubTotalPrice × CommissionRate) - DiscountAmount
    
N?u có Coupon Merchant (GymOwner/FreelancePT):
    System Profit = TotalAmount × CommissionRate
```

#### 2. Merchant Profit (L?i nhu?n merchant)
```
Merchant Profit = SubTotalPrice - System Commission

N?u có Coupon Merchant:
    Merchant Profit = SubTotalPrice - System Commission - DiscountAmount
    
N?u có Coupon System:
    Merchant Profit = SubTotalPrice - System Commission
    (Discount do System ch?u)
```

#### 3. Wallet Flow
```
Khi thanh toán thành công:
    Wallet.PendingBalance += Merchant Profit
    
Khi ??n ProfitDistributePlannedDate:
    Wallet.PendingBalance -= Merchant Profit
    Wallet.AvailableBalance += Merchant Profit
    OrderItem.ProfitDistributeActualDate = Today
```

---

## ?? NHÓM A: TEST MUA GYM COURSE

### TC-GYM-001: Mua Gym Course Không Có PT
**M?c ?ích:** Ki?m tra lu?ng ti?n khi mua gym course c? b?n (không có PT)

**Preconditions:**
```yaml
GymCourse:
  - Id: gym-course-001
  - Name: "Basic Gym Package"
  - Price: 1,000,000 VN?
  - PtPrice: 0 VN?
  - Duration: 30 days

System Config:
  - CommissionRate: 10%
  - ProfitDistributionDays: 7 days

GymOwner:
  - Id: gym-owner-001
  - Wallet.PendingBalance: 0 VN?
  - Wallet.AvailableBalance: 500,000 VN?

Customer:
  - Id: customer-001
  - Wallet.AvailableBalance: 5,000,000 VN?
```

**Test Steps:**
1. Customer t?o order v?i GymCourseId = "gym-course-001"
2. Không ch?n GymPtId (null)
3. Không apply coupon
4. G?i API CreatePaymentLink
5. Thanh toán qua PayOS: 1,000,000 VN?
6. Webhook success ? TransactionsService.PurchaseGymCourse()

**Expected Results:**
```yaml
Order:
  - SubTotalPrice: 1,000,000
  - TotalAmount: 1,000,000
  - Status: Finished
  - CouponId: null

Transaction:
  - TransactionType: GymCourse
  - Status: Success
  - Amount: 1,000,000
  - OrderCode: [generated]
  - ProfitAmount: 900,000

System:
  - Commission: 1,000,000 × 10% = 100,000 VN?

GymOwner Wallet:
  - PendingBalance: 0 + 900,000 = 900,000 VN?
  - AvailableBalance: 500,000 VN? (không ??i)

CustomerPurchased:
  - CustomerId: customer-001
  - AvailableSessions: 0 (không có PT)
  - ExpirationDate: Today + 30 days

OrderItem:
  - GymCourseId: gym-course-001
  - GymPtId: null
  - Quantity: 1
  - Price: 1,000,000
  - ProfitDistributePlannedDate: Today + 7 days

Jobs Scheduled:
  - ProfitDistribution_[orderItemId]: Today + 7 days
  - AutoMarkAsFeedback_[orderItemId]: Today + 7 + autoMarkDays
```

**Verification Points:**
- ? Order.Status = Finished
- ? GymOwner.Wallet.PendingBalance t?ng ?úng 900,000
- ? CustomerPurchased ???c t?o v?i AvailableSessions = 0
- ? ProfitDistributionJob ???c schedule ?úng ngày
- ? Transaction.ProfitAmount = 900,000

---

### TC-GYM-002: Mua Gym Course Có PT
**M?c ?ích:** Ki?m tra lu?ng ti?n khi mua gym course kèm PT

**Preconditions:**
```yaml
GymCourse:
  - Price: 1,000,000 VN?
  - PtPrice: 500,000 VN?
  - Duration: 30 days

GymCoursePT:
  - GymCourseId: gym-course-001
  - PTId: gym-pt-001
  - Session: 12 sessions

GymPT:
  - Id: gym-pt-001
  - PtCurrentCourse: 2
  - PtMaxCourse: 5

System Config:
  - CommissionRate: 10%
  - ProfitDistributionDays: 7 days
```

**Test Steps:**
1. Customer ch?n GymCourseId = "gym-course-001"
2. Ch?n GymPtId = "gym-pt-001"
3. Thanh toán: 1,000,000 + 500,000 = 1,500,000 VN?
4. Webhook success

**Expected Results:**
```yaml
Order:
  - SubTotalPrice: 1,500,000
  - TotalAmount: 1,500,000

Transaction:
  - Amount: 1,500,000
  - ProfitAmount: 1,350,000

System Commission:
  - 1,500,000 × 10% = 150,000 VN?

GymOwner Profit:
  - 1,500,000 - 150,000 = 1,350,000 VN?

GymOwner Wallet:
  - PendingBalance += 1,350,000

GymPT:
  - PtCurrentCourse: 2 ? 3 (t?ng 1)

CustomerPurchased:
  - AvailableSessions: 12
  - ExpirationDate: Today + 30 days

OrderItem:
  - GymCourseId: gym-course-001
  - GymPtId: gym-pt-001
  - Price: 1,500,000
  - ProfitDistributePlannedDate: Today + 7 days

Jobs Scheduled:
  - ProfitDistribution_[orderItemId]: Today + 7 days
  - AutoUpdatePTCurrentCourse_[orderItemId]: ExpirationDate
  - AutoMarkAsFeedback_[orderItemId]: Today + 7 + markDays
```

**Verification Points:**
- ? GymPT.PtCurrentCourse t?ng 1
- ? CustomerPurchased.AvailableSessions = 12
- ? AutoUpdatePTCurrentCourseJob ???c schedule
- ? GymOwner.Wallet.PendingBalance += 1,350,000

---

### TC-GYM-003: Mua Gym Course V?i System Coupon 20%
**M?c ?ích:** Ki?m tra ?nh h??ng c?a System Coupon lên phân ph?i ti?n

**Preconditions:**
```yaml
GymCourse:
  - Price: 1,000,000 VN?

Coupon:
  - Code: "SYSTEM20"
  - Type: System
  - DiscountPercent: 20%
  - MaxDiscount: 300,000 VN?
  - Quantity: 10

System Config:
  - CommissionRate: 10%
```

**Test Steps:**
1. Customer apply coupon "SYSTEM20"
2. Validate coupon success
3. Tính TotalAmount = 1,000,000 - 200,000 = 800,000 VN?
4. Thanh toán 800,000 VN?
5. Webhook success

**Expected Results:**
```yaml
Order:
  - SubTotalPrice: 1,000,000
  - TotalAmount: 800,000
  - CouponId: [coupon-id]

Transaction:
  - Amount: 800,000
  
Discount Calculation:
  - DiscountAmount: Min(1,000,000 × 20%, 300,000) = 200,000 VN?

System Commission:
  - Base: 1,000,000 × 10% = 100,000
  - After Coupon: 100,000 - 200,000 = -100,000 VN?
  - ?? System ch?u l? 100,000 VN?

GymOwner Profit:
  - 1,000,000 - 100,000 = 900,000 VN?
  - (GymOwner KHÔNG ch?u discount)

Transaction.ProfitAmount: 900,000

GymOwner Wallet:
  - PendingBalance += 900,000

Coupon:
  - Quantity: 10 ? 9
  - NumberOfUsedCoupon: 0 ? 1
```

**Verification Points:**
- ? Customer tr? 800,000 thay vì 1,000,000
- ? GymOwner nh?n ?? 900,000 (không b? tr? discount)
- ? System Commission = -100,000 (ch?u l?)
- ? Coupon.Quantity gi?m 1

---

### TC-GYM-004: Mua Gym Course V?i GymOwner Coupon 15%
**M?c ?ích:** Ki?m tra Merchant Coupon - GymOwner t? t?o

**Preconditions:**
```yaml
GymCourse:
  - Price: 1,000,000 VN?
  - GymOwnerId: gym-owner-001

Coupon:
  - Code: "GYMOWNER15"
  - Type: GymOwner
  - DiscountPercent: 15%
  - MaxDiscount: 200,000 VN?
  - CreatorId: gym-owner-001

System Config:
  - CommissionRate: 10%
```

**Test Steps:**
1. Customer apply "GYMOWNER15"
2. Validate: Coupon.Type = GymOwner ?
3. TotalAmount = 1,000,000 - 150,000 = 850,000
4. Thanh toán 850,000
5. Webhook success

**Expected Results:**
```yaml
Order:
  - SubTotalPrice: 1,000,000
  - TotalAmount: 850,000

Discount:
  - 1,000,000 × 15% = 150,000 VN?

System Commission:
  - 850,000 × 10% = 85,000 VN?
  - (Commission tính trên TotalAmount sau discount)

GymOwner Profit:
  - SubTotal - Commission - Discount
  - 1,000,000 - 100,000 - 150,000 = 750,000 VN?
  - (GymOwner ch?u discount)

Transaction.ProfitAmount: 750,000

GymOwner Wallet:
  - PendingBalance += 750,000

Coupon:
  - Quantity gi?m 1
  - NumberOfUsedCoupon t?ng 1
```

**Verification Points:**
- ? Customer tr? 850,000
- ? System Commission = 85,000 (10% c?a TotalAmount)
- ? GymOwner Profit = 750,000 (ch?u discount)
- ? Transaction.ProfitAmount = 750,000

---

### TC-GYM-005: Mua Gym Course - PT ??t Max Course
**M?c ?ích:** Ki?m tra validation khi PT ??t gi?i h?n khóa h?c

**Preconditions:**
```yaml
GymPT:
  - Id: gym-pt-001
  - PtCurrentCourse: 5
  - PtMaxCourse: 5 (?ã ??y)
```

**Test Steps:**
1. Customer ch?n GymCourseId + GymPtId = "gym-pt-001"
2. G?i CreatePaymentLink

**Expected Results:**
```yaml
Error:
  - Type: BusinessException
  - Message: "Maximum course count reached for PT [FullName], current: 5, max: 5"
  
Order:
  - NOT CREATED

Transaction:
  - NOT CREATED

GymPT:
  - PtCurrentCourse: 5 (không ??i)
```

**Verification Points:**
- ? Exception thrown v?i message ?úng
- ? Không t?o Order
- ? PtCurrentCourse không thay ??i

---

## ?? NHÓM B: TEST MUA FREELANCE PT PACKAGE

### TC-FPT-001: Mua Freelance PT Package
**M?c ?ích:** Ki?m tra lu?ng ti?n c? b?n khi mua package

**Preconditions:**
```yaml
FreelancePTPackage:
  - Id: package-001
  - Name: "Premium PT Package"
  - Price: 2,000,000 VN?
  - NumOfSessions: 8
  - DurationInDays: 30
  - PtId: freelance-pt-001

FreelancePT:
  - Id: freelance-pt-001
  - PtCurrentCourse: 1
  - PtMaxCourse: 5

System Config:
  - CommissionRate: 10%
  - AutoMarkAsFeedbackAfterDays: 3
```

**Test Steps:**
1. Customer ch?n FreelancePTPackageId = "package-001"
2. Thanh toán 2,000,000 VN?
3. Webhook success ? PurchaseFreelancePTPackage()

**Expected Results:**
```yaml
Order:
  - SubTotalPrice: 2,000,000
  - TotalAmount: 2,000,000
  - Status: Finished

Transaction:
  - TransactionType: FreelancePTPackage
  - Amount: 2,000,000
  - ProfitAmount: 1,800,000

System Commission:
  - 2,000,000 × 10% = 200,000 VN?

FreelancePT Profit:
  - 2,000,000 - 200,000 = 1,800,000 VN?

FreelancePT Wallet:
  - PendingBalance += 1,800,000

FreelancePT:
  - PtCurrentCourse: 1 ? 2

CustomerPurchased:
  - CustomerId: customer-001
  - AvailableSessions: 8
  - ExpirationDate: Today + 30 days

OrderItem:
  - FreelancePTPackageId: package-001
  - Quantity: 1
  - Price: 2,000,000
  - ProfitDistributePlannedDate: ExpirationDate + 1 day

Jobs Scheduled:
  - ProfitDistribution_[orderItemId]: ExpirationDate + 1
  - AutoUpdatePTCurrentCourse_[orderItemId]: ExpirationDate
  - AutoMarkAsFeedback_[orderItemId]: (ExpirationDate + 1) + 3 days
```

**Verification Points:**
- ? FreelancePT.PtCurrentCourse t?ng 1
- ? ProfitDistributePlannedDate = ExpirationDate + 1 day
- ? CustomerPurchased.AvailableSessions = 8
- ? Wallet.PendingBalance += 1,800,000
- ? All jobs scheduled correctly

---

### TC-FPT-002: Phân Ph?i L?i Nhu?n S?m - 50% Sessions
**M?c ?ích:** Ki?m tra early profit distribution khi hoàn thành ? 50% sessions

**Preconditions:**
```yaml
CustomerPurchased:
  - Id: cp-001
  - AvailableSessions: 8 (g?c)
  - ExpirationDate: Today + 20 days (còn 20 ngày)

OrderItem:
  - Quantity: 1
  - FreelancePTPackage.NumOfSessions: 8
  - ProfitDistributePlannedDate: ExpirationDate + 1
  - ProfitDistributeActualDate: null

Bookings:
  - Total: 8 bookings
  - SessionStatus = Finished: 4 bookings
  - SessionStatus = Booked: 4 bookings

FreelancePT Wallet:
  - PendingBalance: 1,800,000
  - AvailableBalance: 500,000
```

**Test Steps:**
1. Customer hoàn thành booking th? 4
2. G?i DistributePendingProfit(CustomerPurchasedId)
3. Tính: NumOfSessionForDistribute = Ceiling(8 / 2.0) = 4
4. Check: finishedBookings (4) >= allocatedSessions (4) ?
5. RescheduleJob("ProfitDistribution_[orderItemId]", DateTime.UtcNow)

**Expected Results:**
```yaml
Calculation:
  - finishedBookings: 4
  - NumOfSessionForDistribute: Ceiling(8 / 2) = 4
  - allocatedSessionsForDistribute: 4
  - Condition: 4 >= 4 ? TRUE

OrderItem Update:
  - ProfitDistributePlannedDate: Today + 1 day (updated)
  - UpdatedAt: DateTime.UtcNow

Job Rescheduled:
  - Job: "ProfitDistribution_[orderItemId]"
  - Group: "ProfitDistribution"
  - NewTriggerTime: DateTime.UtcNow
  - State: Normal ? Triggered

After 1 Day (Job Runs):
  - FreelancePT Wallet:
      * PendingBalance: 1,800,000 ? 0
      * AvailableBalance: 500,000 ? 2,300,000
  - OrderItem:
      * ProfitDistributeActualDate: Today
  - Transaction:
      * Type: DistributeProfit
      * Amount: 1,800,000
      * Status: Success
```

**Verification Points:**
- ? Condition (4 >= 4) ?úng
- ? Job ???c reschedule v? now
- ? ProfitDistributePlannedDate updated
- ? Sau 1 ngày: PendingBalance ? AvailableBalance
- ? OrderItem.ProfitDistributeActualDate ???c set

---

### TC-FPT-003: Extend Freelance PT Package
**M?c ?ích:** Ki?m tra extend package ch?a h?t h?n

**Preconditions:**
```yaml
CustomerPurchased (Existing):
  - Id: cp-001
  - AvailableSessions: 3 (còn 3 sessions)
  - ExpirationDate: Today + 15 days
  - OrderItems: [orderItem-001] (g?c)

OrderItem-001:
  - Quantity: 1
  - FreelancePTPackageId: package-001
  - CreatedAt: 15 days ago

Extend Request:
  - CustomerPurchasedIdToExtend: cp-001
  - FreelancePTPackageId: package-001
  - Quantity: 2

FreelancePTPackage:
  - Price: 2,000,000
  - NumOfSessions: 8
  - DurationInDays: 30
```

**Test Steps:**
1. Customer g?i ExtendFreelancePTPackage
2. Thanh toán: 2 × 2,000,000 = 4,000,000 VN?
3. Webhook success ? ExtendFreelancePTPackage()

**Expected Results:**
```yaml
Order:
  - SubTotalPrice: 4,000,000
  - TotalAmount: 4,000,000
  - CustomerPurchasedIdToExtend: cp-001

Transaction:
  - TransactionType: ExtendFreelancePTPackage
  - Amount: 4,000,000
  - ProfitAmount: 3,600,000

System Commission:
  - 4,000,000 × 10% = 400,000

FreelancePT Profit:
  - 4,000,000 - 400,000 = 3,600,000

CustomerPurchased (Updated):
  - AvailableSessions: 3 + (2 × 8) = 19
  - ExpirationDate: (Today + 15) + (30 × 2) = Today + 75 days
  - OrderItems: [orderItem-001, orderItem-002]

OrderItem-002 (New):
  - CustomerPurchasedId: cp-001
  - Quantity: 2
  - Price: 2,000,000
  - ProfitDistributePlannedDate: NEW_ExpirationDate + 1

FreelancePT Wallet:
  - PendingBalance += 3,600,000

Jobs:
  - NEW: ProfitDistribution_[orderItem-002]: (Today + 75) + 1
  - RESCHEDULED: AutoUpdatePTCurrentCourse_[orderItem-001]: Today + 75
  - NEW: AutoMarkAsFeedback_[orderItem-002]: (Today + 76) + 3
```

**Verification Points:**
- ? KHÔNG t?o CustomerPurchased m?i
- ? AvailableSessions c?ng d?n ?úng: 3 + 16 = 19
- ? ExpirationDate ???c extend: +60 days
- ? ProfitDistributePlannedDate = NEW_ExpirationDate + 1
- ? AutoUpdatePTCurrentCourse ???c reschedule
- ? Wallet.PendingBalance += 3,600,000

---

### TC-FPT-004: Extend Package V?i Coupon 30%
**M?c ?ích:** Ki?m tra extend package khi có coupon

**Preconditions:**
```yaml
CustomerPurchased:
  - AvailableSessions: 5
  - ExpirationDate: Today + 10 days

Extend:
  - Quantity: 1
  - Price: 2,000,000

Coupon:
  - Code: "PT30"
  - Type: FreelancePT
  - DiscountPercent: 30%
  - MaxDiscount: 800,000
```

**Test Steps:**
1. Apply coupon "PT30"
2. TotalAmount = 2,000,000 - 600,000 = 1,400,000
3. Thanh toán 1,400,000
4. Webhook success

**Expected Results:**
```yaml
Order:
  - SubTotalPrice: 2,000,000
  - TotalAmount: 1,400,000

Discount:
  - 2,000,000 × 30% = 600,000

System Commission:
  - 1,400,000 × 10% = 140,000

FreelancePT Profit:
  - 2,000,000 - 200,000 - 600,000 = 1,200,000
  - (PT ch?u discount)

Wallet:
  - PendingBalance += 1,200,000

CustomerPurchased:
  - AvailableSessions: 5 + 8 = 13
  - ExpirationDate: (Today + 10) + 30 = Today + 40

Coupon:
  - Quantity gi?m 1
```

**Verification Points:**
- ? Customer tr? 1,400,000 thay vì 2,000,000
- ? PT profit = 1,200,000 (ch?u discount)
- ? System commission = 140,000
- ? Sessions và ExpirationDate updated ?úng

---

## ?? NHÓM C: TEST EXTEND GYM COURSE

### TC-EXT-GYM-001: Extend Gym Course Có PT
**M?c ?ích:** Ki?m tra extend gym course v?i PT

**Preconditions:**
```yaml
CustomerPurchased:
  - Id: cp-001
  - AvailableSessions: 5
  - ExpirationDate: Today + 10 days
  - OrderItems: [orderItem-001]

GymCourse:
  - Price: 1,000,000
  - PtPrice: 500,000
  - Duration: 30

GymCoursePT:
  - Session: 12

Extend Request:
  - CustomerPurchasedIdToExtend: cp-001
  - Quantity: 1

System Config:
  - ProfitDistributionDays: 7
```

**Test Steps:**
1. Customer extend v?i Quantity = 1
2. Thanh toán: 1,000,000 + 500,000 = 1,500,000
3. Webhook success ? ExtendGymCourse()

**Expected Results:**
```yaml
Order:
  - SubTotalPrice: 1,500,000
  - TotalAmount: 1,500,000
  - CustomerPurchasedIdToExtend: cp-001

Transaction:
  - TransactionType: ExtendCourse
  - Amount: 1,500,000
  - ProfitAmount: 1,350,000

CustomerPurchased (Updated):
  - AvailableSessions: 5 + 12 = 17
  - ExpirationDate: (Today + 10) + 30 = Today + 40
  - KHÔNG t?o m?i

OrderItem (New):
  - GymCourseId: [gym-course-id]
  - GymPtId: [gym-pt-id]
  - CustomerPurchasedId: cp-001
  - Quantity: 1
  - Price: 1,500,000
  - ProfitDistributePlannedDate: Today + 7 days
    ?? KHÔNG ph?i ExpirationDate + 1

GymOwner Wallet:
  - PendingBalance += 1,350,000

Jobs:
  - NEW: ProfitDistribution_[new-orderItem]: Today + 7
  - RESCHEDULED: AutoUpdatePTCurrentCourse_[orderItem-001]: Today + 40
  - NEW: AutoMarkAsFeedback_[new-orderItem]: (Today + 7) + markDays
```

**Verification Points:**
- ? AvailableSessions: 5 ? 17
- ? ExpirationDate: +30 days
- ? ProfitDistributePlannedDate = Today + 7 (KHÔNG ph?i ExpirationDate + 1)
- ? AutoUpdatePTCurrentCourse ???c reschedule
- ? Wallet.PendingBalance += 1,350,000

---

### TC-EXT-GYM-002: Extend Nhi?u L?n Liên Ti?p
**M?c ?ích:** Ki?m tra extend 3 l?n trong 1 tháng

**Scenario:**
```yaml
Initial State:
  - AvailableSessions: 5
  - ExpirationDate: Day 10

Extend 1 (Day 5):
  - Quantity: 1
  - Sessions: 5 ? 17
  - ExpirationDate: Day 10 ? Day 40
  - ProfitDistributePlannedDate: Day 5 + 7 = Day 12

Extend 2 (Day 15):
  - Quantity: 1
  - Sessions: 17 ? 29
  - ExpirationDate: Day 40 ? Day 70
  - ProfitDistributePlannedDate: Day 15 + 7 = Day 22

Extend 3 (Day 25):
  - Quantity: 1
  - Sessions: 29 ? 41
  - ExpirationDate: Day 70 ? Day 100
  - ProfitDistributePlannedDate: Day 25 + 7 = Day 32
```

**Expected Profit Distribution:**
```yaml
Day 12: Distribute profit from Extend 1
Day 22: Distribute profit from Extend 2
Day 32: Distribute profit from Extend 3

Total Profit Distributions: 3 l?n riêng bi?t
```

**Verification Points:**
- ? 3 OrderItem riêng bi?t
- ? 3 ProfitDistributionJob riêng bi?t
- ? M?i job có ProfitDistributePlannedDate khác nhau
- ? Sessions c?ng d?n ?úng
- ? AutoUpdatePTCurrentCourse reschedule 2 l?n

---

## ?? NHÓM D: TEST PROFIT DISTRIBUTION

### TC-PROFIT-001: Phân Ph?i Ti?n ?úng H?n
**M?c ?ích:** Ki?m tra job DistributeProfit ch?y ?úng scheduled time

**Preconditions:**
```yaml
OrderItem:
  - Id: oi-001
  - ProfitDistributePlannedDate: 2025-01-10
  - ProfitDistributeActualDate: null

Order:
  - Coupon: null

FreelancePT Wallet:
  - PendingBalance: 1,800,000
  - AvailableBalance: 500,000

FreelancePTPackage:
  - Price: 2,000,000
  - PtId: pt-001

System Config:
  - CommissionRate: 10%
```

**Test Steps:**
1. Time reach 2025-01-10 00:00:00
2. DistributeProfitJob triggers
3. Execute DistributeProfit(orderItemId: oi-001)

**Expected Results:**
```yaml
Profit Calculation:
  - SubTotalOrderItemPrice: 2,000,000 × 1 = 2,000,000
  - Commission: 2,000,000 × 10% = 200,000
  - Merchant Profit: 2,000,000 - 200,000 = 1,800,000

Transaction (New):
  - Type: DistributeProfit
  - Amount: 1,800,000
  - WalletId: pt-001
  - OrderId: [order-id]
  - OrderItemId: oi-001
  - Status: Success
  - OrderCode: [generated]
  - PaymentMethodId: [System]

FreelancePT Wallet (Updated):
  - PendingBalance: 1,800,000 ? 0
  - AvailableBalance: 500,000 ? 2,300,000

OrderItem (Updated):
  - ProfitDistributeActualDate: 2025-01-10
  - UpdatedAt: 2025-01-10 00:00:00

Logs:
  - "Wallet pt-001 updated with available balance 500,000 plus 1,800,000 
     and pending balance 1,800,000 minus 1,800,000"
```

**Verification Points:**
- ? Job ch?y ?úng lúc 2025-01-10
- ? PendingBalance gi?m 1,800,000
- ? AvailableBalance t?ng 1,800,000
- ? ProfitDistributeActualDate ???c set
- ? Transaction DistributeProfit ???c t?o
- ? Log ghi ?úng thông tin

---

### TC-PROFIT-002: Phân Ph?i S?m - 75% Sessions
**M?c ?ích:** Ki?m tra early distribution v?i nhi?u h?n 50% sessions

**Preconditions:**
```yaml
CustomerPurchased:
  - AvailableSessions: 10 (g?c)
  - OrderItems: [oi-001, oi-002]

OrderItem-001:
  - Quantity: 1
  - NumOfSessions: 10
  - ProfitDistributeActualDate: null

Bookings:
  - SessionStatus = Finished: 8 bookings
  - SessionStatus = Booked: 2 bookings
```

**Test Steps:**
1. Customer hoàn thành booking th? 8
2. G?i DistributePendingProfit()

**Expected Results:**
```yaml
Calculation:
  - NumOfSessionForDistribute: Ceiling(10 / 2) = 5
  - finishedBookings: 8
  - allocatedSessions: 5
  - Condition: 8 >= 5 ? TRUE

Action:
  - RescheduleJob("ProfitDistribution_oi-001", DateTime.UtcNow)
  - ProfitDistributePlannedDate: Tomorrow

Result:
  - Profit distributed s?m h?n ExpirationDate + 1
  - Wallet.AvailableBalance t?ng ngay
```

**Verification Points:**
- ? 8 bookings > 5 required ? trigger
- ? Job reschedule thành công
- ? Profit ???c phân ph?i s?m

---

### TC-PROFIT-003: Phân Ph?i V?i Nhi?u OrderItem
**M?c ?ích:** Ki?m tra DistributePendingProfit v?i nhi?u extend

**Preconditions:**
```yaml
CustomerPurchased:
  - OrderItems: 
      * oi-001: Quantity=1, NumOfSessions=8 (g?c)
      * oi-002: Quantity=2, NumOfSessions=8 (extend 1)
      * oi-003: Quantity=1, NumOfSessions=8 (extend 2)

Bookings:
  - SessionStatus = Finished: 12 bookings
```

**Test Steps:**
1. G?i DistributePendingProfit()

**Expected Results:**
```yaml
Loop Through OrderItems (Ordered by CreatedAt):

OrderItem-001:
  - NumOfSessionForDistribute: Ceiling(1 × 8 / 2) = 4
  - allocatedSessions: 4
  - finishedBookings: 12
  - Condition: 12 >= 4 ?
  - Action: Reschedule oi-001

OrderItem-002:
  - NumOfSessionForDistribute: Ceiling(2 × 8 / 2) = 8
  - allocatedSessions: 4 + 8 = 12
  - finishedBookings: 12
  - Condition: 12 >= 12 ?
  - Action: Reschedule oi-002

OrderItem-003:
  - NumOfSessionForDistribute: Ceiling(1 × 8 / 2) = 4
  - allocatedSessions: 12 + 4 = 16
  - finishedBookings: 12
  - Condition: 12 >= 16 ?
  - Action: SKIP (ch?a ?? sessions)

Result:
  - oi-001 và oi-002 ???c phân ph?i
  - oi-003 ch? thêm 4 bookings n?a
```

**Verification Points:**
- ? Loop ?úng th? t? (CreatedAt)
- ? allocatedSessions c?ng d?n
- ? oi-001 và oi-002 ???c reschedule
- ? oi-003 b? skip (ch?a ??)

---

## ?? NHÓM E: TEST COUPON IMPACT

### TC-COUPON-001: System Coupon 50% (Max 500k) - Order 2M
**M?c ?ích:** Ki?m tra System Coupon v?i MaxDiscount

**Preconditions:**
```yaml
Order:
  - SubTotalPrice: 2,000,000

Coupon:
  - Type: System
  - DiscountPercent: 50%
  - MaxDiscount: 500,000

System Config:
  - CommissionRate: 10%
```

**Test Steps:**
1. Apply coupon
2. Calculate discount
3. Process payment

**Expected Results:**
```yaml
Discount Calculation:
  - Raw: 2,000,000 × 50% = 1,000,000
  - Applied: Min(1,000,000, 500,000) = 500,000 (capped)

Order:
  - SubTotalPrice: 2,000,000
  - TotalAmount: 1,500,000

System Commission:
  - Base: 2,000,000 × 10% = 200,000
  - After Coupon: 200,000 - 500,000 = -300,000
  - ?? System L? 300,000

Merchant Profit:
  - 2,000,000 - 200,000 = 1,800,000
  - (Merchant KHÔNG ch?u discount)

Transaction.ProfitAmount: 1,800,000

Wallet:
  - PendingBalance += 1,800,000
```

**Verification Points:**
- ? Discount capped t?i MaxDiscount
- ? System commission âm (ch?u l?)
- ? Merchant profit không b? ?nh h??ng
- ? Customer tr? 1,500,000

---

### TC-COUPON-002: Merchant Coupon 30% - Order 1M
**M?c ?ích:** Ki?m tra Merchant Coupon

**Preconditions:**
```yaml
Order:
  - SubTotalPrice: 1,000,000

Coupon:
  - Type: FreelancePT
  - DiscountPercent: 30%
  - MaxDiscount: 500,000

System Config:
  - CommissionRate: 10%
```

**Test Steps:**
1. Apply FreelancePT coupon
2. Calculate

**Expected Results:**
```yaml
Discount:
  - 1,000,000 × 30% = 300,000

Order:
  - SubTotalPrice: 1,000,000
  - TotalAmount: 700,000

System Commission:
  - 700,000 × 10% = 70,000
  - (Commission trên TotalAmount)

Merchant Profit:
  - 1,000,000 - 100,000 - 300,000 = 600,000
  - (Merchant CH?U discount)

Transaction.ProfitAmount: 600,000

Wallet:
  - PendingBalance += 600,000
```

**Verification Points:**
- ? Customer tr? 700,000
- ? System commission = 70,000 (10% c?a 700k)
- ? Merchant profit = 600,000
- ? Discount do Merchant ch?u

---

### TC-COUPON-003: Coupon H?t S? L??ng
**M?c ?ích:** Validate khi coupon h?t

**Preconditions:**
```yaml
Coupon:
  - Quantity: 0
  - NumberOfUsedCoupon: 100
```

**Test Steps:**
1. Customer apply coupon code

**Expected Results:**
```yaml
Error:
  - Type: BusinessException
  - Message: "Coupon out of stock" ho?c t??ng t?

Order:
  - NOT CREATED
```

**Verification Points:**
- ? Exception thrown
- ? Không t?o order
- ? Coupon.Quantity không thay ??i

---

## ?? NHÓM F: TEST EDGE CASES

### TC-EDGE-001: Extend Package ?ã H?t H?n
**M?c ?ích:** Không cho phép extend package expired

**Preconditions:**
```yaml
CustomerPurchased:
  - ExpirationDate: 2024-12-01
  - Today: 2024-12-15 (?ã quá 14 ngày)
```

**Test Steps:**
1. Customer c? g?ng extend package

**Expected Results:**
```yaml
Error:
  - ? PackageExpiredException ho?c BusinessException
  - Message: "Package already expired. Please purchase new package."

Action:
  - Customer ph?i mua m?i (KHÔNG extend)
```

**Verification Points:**
- ? Không cho extend package expired
- ? Exception message rõ ràng
- ? Suggest mua m?i

---

### TC-EDGE-002: PT ??t MaxCourse
**M?c ?ích:** Prevent purchase khi PT full

**Preconditions:**
```yaml
FreelancePT:
  - PtCurrentCourse: 5
  - PtMaxCourse: 5
```

**Test Steps:**
1. Customer mua package c?a PT này

**Expected Results:**
```yaml
Error:
  - Type: BusinessException
  - Message: "Maximum course count reached for freelance PT [FullName], 
             current course count: 5, maximum course count: 5"

FreelancePT:
  - PtCurrentCourse: 5 (không ??i)
```

**Verification Points:**
- ? Validation tr??c khi t?o order
- ? PtCurrentCourse không thay ??i
- ? Message ch?a tên PT và s? li?u

---

### TC-EDGE-003: Package Còn T?n T?i - Không Cho Mua M?i
**M?c ?ích:** Prevent duplicate package

**Preconditions:**
```yaml
CustomerPurchased (Existing):
  - FreelancePTId: pt-001
  - CustomerId: customer-001
  - ExpirationDate: Today + 20 days (còn h?n)
```

**Test Steps:**
1. Customer c? mua l?i package c?a cùng PT

**Expected Results:**
```yaml
Error:
  - Type: PackageExistException
  - Message: "Package of this freelance PT still not expired, 
             customer purchased id: [id], 
             package expiration date: [date] 
             please extend the package"

Action:
  - Customer ph?i extend (KHÔNG mua m?i)
```

**Verification Points:**
- ? Validation check existing package
- ? Suggest extend thay vì mua m?i
- ? Message ch?a CustomerPurchasedId và ExpirationDate

---

### TC-EDGE-004: Profit Distribution Job B? Pause
**M?c ?ích:** Ki?m tra khi job ?ã b? pause

**Preconditions:**
```yaml
OrderItem:
  - Id: oi-001
  - ProfitDistributePlannedDate: Today + 5

Job Status:
  - State: Paused (b? admin pause)

Bookings:
  - finishedBookings: 5 (?? ?? early distribute)
```

**Test Steps:**
1. G?i DistributePendingProfit()
2. Check job status = Paused

**Expected Results:**
```yaml
Action:
  - SKIP reschedule
  - Log: "Profit distribution job for order item oi-001 is already paused"

OrderItem:
  - ProfitDistributePlannedDate: không ??i
  - ProfitDistributeActualDate: null

Job:
  - State: Paused (v?n pause)
```

**Verification Points:**
- ? Không reschedule job ?ang pause
- ? Log ghi nh?n job paused
- ? Không throw exception

---

## ?? B?NG T?NG H?P COMMISSION

| Lo?i Transaction | Coupon Type | System Profit | Merchant Profit |
|------------------|-------------|---------------|-----------------|
| **Purchase Gym Course** | None | SubTotal × 10% | SubTotal - Commission |
| **Purchase Gym Course** | System 20% | (SubTotal × 10%) - Discount | SubTotal - Commission |
| **Purchase Gym Course** | GymOwner 15% | TotalAmount × 10% | SubTotal - Commission - Discount |
| **Purchase Freelance PT** | None | SubTotal × 10% | SubTotal - Commission |
| **Purchase Freelance PT** | FreelancePT 30% | TotalAmount × 10% | SubTotal - Commission - Discount |
| **Extend Gym Course** | None | SubTotal × 10% | SubTotal - Commission |
| **Extend Freelance PT** | None | SubTotal × 10% | SubTotal - Commission |

---

## ?? PRIORITY TEST MATRIX

| Priority | Category | Test Cases | Execution Time |
|----------|----------|------------|----------------|
| **P0** | Mua course c? b?n | TC-GYM-001, TC-GYM-002, TC-FPT-001 | 30 min |
| **P0** | Profit distribution | TC-PROFIT-001, TC-PROFIT-002 | 20 min |
| **P1** | Extend course | TC-EXT-GYM-001, TC-FPT-003 | 25 min |
| **P1** | Coupon impact | TC-COUPON-001, TC-COUPON-002, TC-GYM-003, TC-GYM-004 | 30 min |
| **P2** | Edge cases | TC-EDGE-001, TC-EDGE-002, TC-EDGE-003, TC-EDGE-004 | 25 min |
| **P2** | Multiple extends | TC-EXT-GYM-002, TC-PROFIT-003 | 20 min |
| **Total** | | **18 Test Cases** | **~150 min** |

---

## ?? ?I?M ??C BI?T C?N L?U Ý

### 1. **ProfitDistributePlannedDate Khác Nhau**
```yaml
Gym Course Purchase:
  - ProfitDistributePlannedDate: Today + ProfitDistributionDays (7 days)

Freelance PT Purchase:
  - ProfitDistributePlannedDate: ExpirationDate + 1 day

Extend Gym Course:
  - ProfitDistributePlannedDate: Today + ProfitDistributionDays (7 days)
  - ?? KHÔNG ph?i ExpirationDate + 1

Extend Freelance PT:
  - ProfitDistributePlannedDate: NEW_ExpirationDate + 1 day
```

### 2. **Early Distribution (Ch? Freelance PT)**
```yaml
Trigger Condition:
  - finishedBookings >= Ceiling(NumOfSessions × Quantity / 2.0)

Action:
  - RescheduleJob("ProfitDistribution_[orderItemId]", DateTime.UtcNow)
  - Update ProfitDistributePlannedDate = Tomorrow

Note:
  - Ch? áp d?ng cho Freelance PT Package
  - Gym Course KHÔNG có early distribution
```

### 3. **PT Current Course Management**
```yaml
T?ng PtCurrentCourse:
  - Khi Purchase Gym Course có PT
  - Khi Purchase Freelance PT Package
  - ?? KHÔNG t?ng khi Extend

Gi?m PtCurrentCourse:
  - AutoUpdatePTCurrentCourseJob ch?y at ExpirationDate
```

### 4. **Commission Calculation Flow**
```csharp
// Base calculation
SystemProfit = SubTotalPrice × CommissionRate;

// If System Coupon
if (Coupon.Type == CouponType.System) {
    SystemProfit -= DiscountAmount;  // System ch?u discount
}

// If Merchant Coupon
if (Coupon.Type != CouponType.System) {
    SystemProfit = TotalAmount × CommissionRate;  // Commission sau discount
}

// Merchant Profit
MerchantProfit = SubTotalPrice - SystemProfit;

if (Coupon.Type != CouponType.System) {
    MerchantProfit -= DiscountAmount;  // Merchant ch?u discount
}
```

---

## ?? TEST DATA SETUP

### Tài Kho?n Test
```yaml
Gym Owner:
  - Email: gymowner@test.com
  - Wallet: 1,000,000 VN?

Freelance PT:
  - Email: freelancept@test.com
  - Wallet: 500,000 VN?
  - PtMaxCourse: 5

Gym PT:
  - Email: gympt@test.com
  - PtMaxCourse: 5

Customer:
  - Email: customer@test.com
  - Wallet: 10,000,000 VN?
```

### System Config
```yaml
CommissionRate: 10%
ProfitDistributionDays: 7
AutoMarkAsFeedbackAfterDays: 3
```

### Coupons
```yaml
SYSTEM20:
  - Type: System
  - Discount: 20%
  - Max: 300,000
  - Quantity: 100

GYMOWNER15:
  - Type: GymOwner
  - Discount: 15%
  - Max: 200,000
  - Quantity: 50

PT30:
  - Type: FreelancePT
  - Discount: 30%
  - Max: 800,000
  - Quantity: 50
```

---

## ?? TEST EXECUTION REPORT TEMPLATE

```markdown
# Test Execution Report - Money Flow

**Date:** [YYYY-MM-DD]
**Tester:** [Name]
**Environment:** [Dev/Staging/Prod]

## Summary
- Total Test Cases: 18
- Passed: __
- Failed: __
- Blocked: __
- Skipped: __

## P0 Tests (Critical)
| TC ID | Name | Status | Notes |
|-------|------|--------|-------|
| TC-GYM-001 | Mua Gym Course Không PT | ? | |
| TC-GYM-002 | Mua Gym Course Có PT | ? | |
| TC-FPT-001 | Mua Freelance PT Package | ? | |
| TC-PROFIT-001 | Phân Ph?i ?úng H?n | ? | |
| TC-PROFIT-002 | Phân Ph?i S?m 50% | ? | |

## P1 Tests (High)
| TC ID | Name | Status | Notes |
|-------|------|--------|-------|
| TC-EXT-GYM-001 | Extend Gym Course | ? | |
| TC-FPT-003 | Extend Freelance PT | ? | |
| TC-COUPON-001 | System Coupon | ? | |
| TC-COUPON-002 | Merchant Coupon | ? | |
| TC-GYM-003 | System Coupon Gym | ? | |
| TC-GYM-004 | GymOwner Coupon | ? | |

## Issues Found
1. [Issue description]
2. [Issue description]

## Recommendations
- [Recommendation 1]
- [Recommendation 2]
```

---

**Document Version:** 1.0  
**Last Updated:** 2024-12-03  
**Author:** QA Team - FitBridge  
**Status:** ? Ready for Testing
