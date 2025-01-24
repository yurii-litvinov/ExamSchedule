export interface LoginResponse {
    accessToken: string;
    refreshToken: string;
    role: string;
    staff: Staff
}

export interface Staff {
    staffId: number;
    firstName: string;
    lastName: string;
    middleName: string;
    email: string;
    password: string;
}