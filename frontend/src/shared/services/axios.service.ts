import axios from "axios";

const axiosService = axios.create({
    baseURL: "http://localhost:5000/",
    headers: undefined,
    data: undefined
});

export const getExams = () => axiosService.get("api/exams")

export const deleteExam = (id: number) => axiosService.delete(`api/exams/${id}`)