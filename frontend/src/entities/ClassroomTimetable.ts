// Interfaces for classroom timetable
export interface ClassroomEventsDay {
    dayString: string;
    dayStudyEvents: DayStudyEvent[];
}

interface DayStudyEvent {
    subject: string;
    timeIntervalString: string;
    dates: string[];
    contingentUnitNames: ContingentUnitName[];
    educatorsDisplayText: string;
}

interface ContingentUnitName {
    item1: string;
}