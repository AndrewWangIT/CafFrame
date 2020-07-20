using System;

namespace CafApi
{
    public class WeatherForecast
    {
        public DateTime Date { get; set; }

        public int TemperatureC { get; set; }

        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

        public string Summary { get; set; }
    }
    public class AccountOptions
    {
        /// <summary>
        /// 验证码的超时时间
        /// </summary>
        public int ResetPasswordCodeExpire { get; set; }
        /// <summary>
        /// Lock时长
        /// </summary>
        public int LockTime { get; set; }
        /// <summary>
        /// 密码错误次数计算间隔(多长时间后重新计数错误次数)
        /// </summary>
        public int PasswordErrTimeRange { get; set; }
        /// <summary>
        /// 最大错误次数
        /// </summary>
        public int MaxLoginErrCount { get; set; }
        /// <summary>
        /// 密码过期时间
        /// </summary>
        public int PasswordExpireDays { get; set; }
    }
}
