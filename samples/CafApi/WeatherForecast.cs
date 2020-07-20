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
        /// ��֤��ĳ�ʱʱ��
        /// </summary>
        public int ResetPasswordCodeExpire { get; set; }
        /// <summary>
        /// Lockʱ��
        /// </summary>
        public int LockTime { get; set; }
        /// <summary>
        /// ����������������(�೤ʱ������¼����������)
        /// </summary>
        public int PasswordErrTimeRange { get; set; }
        /// <summary>
        /// ���������
        /// </summary>
        public int MaxLoginErrCount { get; set; }
        /// <summary>
        /// �������ʱ��
        /// </summary>
        public int PasswordExpireDays { get; set; }
    }
}
