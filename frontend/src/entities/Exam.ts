// Exam interfaces
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

export interface InputExam {
    title: string;
    type: string;
    studentInitials: string;
    studentGroup: string;
    classroom: string;
    dateTime: string;
    lecturersInitials: string[];
}

export interface Lecturer {
    staffId: number;
    firstName: string;
    lastName: string;
    middleName: string;
    email: string;
}