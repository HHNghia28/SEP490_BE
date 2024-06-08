﻿using BusinessObject.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Interfaces
{
    public interface IAttendanceRepository
    {
        public Task<IEnumerable<AttendenceResponse>> GetAttendenceBySlot(string slotID);
        public Task<IEnumerable<AttendenceResponse>> GetAttendenceStudent(string studentID, string subjectName, string schoolYear);
        public Task<Dictionary<string, Dictionary<string, object>>> GetAttendenceStudentAllSubject(string studentID, string schoolYear);
        public Task UpdateAttendence(string accountID, List<AttendenceRequest> requests);
    }
}
