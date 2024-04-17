import axios from "axios";
import {Exam, InputExam} from "@entities/Exam.ts";

const axiosService = axios.create({
    baseURL: "http://localhost:5000/",
    headers: undefined,
    data: undefined
});

export const getExams = () => axiosService.get("api/exams")

export const deleteExam = (id: number) => axiosService.delete(`api/exams/${id}`)

export const updateExam = (id: number, exam: Exam) => axiosService.put(`api/exams/?examId=${id}`, exam)

export const insertExam = (exam: InputExam) => axiosService.post(`api/exams/`, exam)