// Exam interfaces
export interface InputExam {
    title: string;
    type: string;
    studentInitials: string;
    studentGroup: string;
    classroom: string;
    dateTime: string;
    lecturersInitials: string[];
}

export interface Exam {
    examId: number;
    title: string;
    type: string;
    studentInitials: string;
    studentGroup: string;
    classroom: string;
    dateTime: string;
    isPassed: boolean;
    lecturers: Lecturer[];
}


export interface Lecturer {
    staffId: number;
    firstName: string;
    lastName: string;
    middleName: string;
    email: string;
}