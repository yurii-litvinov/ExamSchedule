export interface Exam {
    examId: number;
    title: string;
    type: string;
    student_initials: string;
    student_group: string;
    classroom: string;
    dateTime: string;
    lecturers: Lecturer[];
}

export interface Lecturer {
    lecturerId: number;
    firstName: string;
    lastName: string;
    middleName: string;
    email: string;
}