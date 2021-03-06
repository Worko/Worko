﻿using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class WorkScheduleTemplate
    {
        

        public Shift[] Shifts { get; set; }

        public WorkScheduleTemplate()
        {
            Shifts = new Shift[21];

            for (int i = 0; i < 21; i++)
            {
                int part = i / 7;
                int day = i - 7 * part;
                Shifts[i] = new Shift() 
                {
                    Day = (DayOfWeek)day,
                    Part = (PartOfDay)part,
                    IsActive = true,
                    Stations = new List<Station>()
                };
            }

            Shifts[Shift.GetShiftIndex(DayOfWeek.Friday, PartOfDay.Evening)].IsActive = false;
            Shifts[Shift.GetShiftIndex(DayOfWeek.Friday, PartOfDay.Night)].IsActive = false;
            Shifts[Shift.GetShiftIndex(DayOfWeek.Saturday, PartOfDay.Morning)].IsActive = false;
            Shifts[Shift.GetShiftIndex(DayOfWeek.Saturday, PartOfDay.Evening)].IsActive = false;
        }
    }
}
