export interface Exam {
    examId: number;
    title: string;
    type: string;
    student_initials: string;
    student_group: string;
    classroom: string;
    dateTime: string;
    isPassed: boolean;
    lecturers: Lecturer[];
}

export interface Lecturer {
    lecturerId: number;
    firstName: string;
    lastName: string;
    middleName: string;
    email: string;
}