using System;
using Restaurant.Templates;

namespace Restaurant.staff
{
    public interface IStaff
    {
        void Work(object obj);
        void StartWork(Order order);
        void FinishWork();
        object GetStaffLock();
        string GetStatusName();
        bool GetStatus();
        DateTime GetTimeOfLastWork();
        int GetId();
    }
}