import axios, {AxiosHeaders} from "axios";
import {InputExam} from "@entities/Exam.ts";
import {authHeader, refreshToken} from "@shared/services/auth.service.ts";
import {setAccessToken, setRefreshToken} from "@shared/services/localStorage.service.ts";

// Axios service for API requesting
export const axiosService = axios.create({
    baseURL: "http://localhost:5000/",
    headers: undefined,
    data: undefined
});

/// Config for axios requests
axiosService.interceptors.request
    .use(function (config) {
        if (config.url?.includes("api/")) {
            config.headers = {...authHeader()} as AxiosHeaders
        }
        return config;
    });

/// Expired token and unauthorized handler
axiosService.interceptors.response
    .use(function (response) {
        return response;
    }, async function (error) {
        const loginUrl = "/login"
        try {
            if (error.response.status === 401) {
                if (error.config.url === "api/refresh/") {
                    setAccessToken("");
                    setRefreshToken("");
                    window.location.assign(loginUrl);
                    return Promise.reject(error);
                }

                await refreshToken()
                return axiosService(error.config);
            }
            return Promise.reject(error);
        } catch {
            window.location.assign(loginUrl);
            return;
        }
    });

export const getExams = () => axiosService.get("api/exams")

export const getExam = (id: number) => axiosService.get(`api/exams/${id}`)

export const getRoleById = (id: number) => axiosService.get(`api/staffs/${id}/role`)

export const getStaff = (id: number) => axiosService.get(`api/staffs/${id}`)

export const login = (email: string, password: string) => axiosService.post(`api/login`, {email, password})

export const deleteExam = (id: number) => axiosService.delete(`api/exams/${id}`)

export const updateExam = (id: number, exam: InputExam) => axiosService.put(`api/exams/?examId=${id}`, exam)

export const insertExam = (exam: InputExam) => axiosService.post(`api/exams/`, exam)

export const getEducatorTimetable = (initials: string) => axiosService.get(`api/timetable/educator/?lecturerInitials=${initials}`)

export const getLocationTimetable = (location: string, startDate: string, endDate: string) => axiosService.get(`api/timetable/location?location=${location}&startDate=${startDate}&endDate=${endDate}`)

export const getGroupTimetable = (groupOid: number, startDate: string) => axiosService.get(`api/timetable/group?studentGroupOid=${groupOid}&startDate=${startDate}`)

export const getReport = (ids: number[]) => axiosService.post(`api/report`, ids, {responseType: 'arraybuffer'})

export const getGroupByTitle = (title: string) => axiosService.get(`api/student_group/${title}`)