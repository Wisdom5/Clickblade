using UnityEngine;

namespace Core
{
    public class PerformanceService
    {
        public void Initialize()
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 90;
        }
    }
}
