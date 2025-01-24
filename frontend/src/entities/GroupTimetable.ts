// Interfaces for group timetable
export interface GroupEventsDay {
    dayString: string;
    dayStudyEvents: GroupDayStudyEvent[];
}

interface GroupDayStudyEvent {
    subject: string;
    timeIntervalString: string;
    locationsDisplayText: string;
    educatorsDisplayText: string;
}