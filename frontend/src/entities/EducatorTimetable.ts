// Interfaces for educator timetable
export interface EducatorEventsDay {
    dayString: string;
    dayStudyEvents: DayStudyEvent[];
}

interface DayStudyEvent {
    subject: string;
    timeIntervalString: string;
    dates: string[];
    contingentUnitNames: ContingentUnitName[];
}

interface ContingentUnitName {
    item1: string;
}