namespace FitBridge_Domain.Enums.Trainings
{
    public enum ActivityType //Loại bài tập ví dụ như là workout, tập với thiết bị, khởi động
    {
        WarmUp,          // khởi động, động tác chuẩn bị
        Resistance,      // tạ/máy/dây -> có sets, reps, weight
        Cardio,          // chạy bộ, đạp xe (thời lượng, quãng đường, pace)
        Mobility,        // giãn cơ/ROM, foam rolling (thời lượng)
        CoolDown,        // thả lỏng kết thúc
        Rehab          // phục hồi/chấn thương (bài tập nhẹ, isometric…)
    }
}