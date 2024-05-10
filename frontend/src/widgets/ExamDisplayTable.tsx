import {Paper, Table, TableBody, TableCell, TableContainer, TableHead, TableRow} from "@mui/material";
import {Exam} from "@entities/Exam";
import React, {useMemo} from "react";
import {deleteExam, updateExam} from "@shared/services/axios.service.ts";
import DeleteIcon from '@mui/icons-material/Delete';
import IconButton from "@mui/material/IconButton";
import SwapVertIcon from '@mui/icons-material/SwapVert';
import moment from 'moment';


interface TableProps {
    data: Exam[],
    setData: React.Dispatch<React.SetStateAction<Exam[]>>,
    onPassedAction: (exam: Exam) => void,
}


// Table for exam display
export const ExamDisplayTable = ({data, setData, onPassedAction}: TableProps) => {
    moment.locale('ru')
    const headers = useMemo(() => ["Cдан/не сдан", "Id", "Студент", "Дисциплина", "Группа", "Тип", "Место проведения", "Дата", "Преподаватели", ""], [])

    const onDeleteClick = (examId: number) => {
        let dataCopy = data;
        deleteExam(examId).then(() => {
            dataCopy = dataCopy.filter(ex => ex.examId != examId)
            setData(dataCopy)
        })
    }

    // Action on change passed status button click
    const onPassedClick = (examId: number) => {
        let dataCopy = data;
        const exam = dataCopy.find(ex => ex.examId == examId)
        if (!exam) return
        exam.isPassed = !exam.isPassed
        updateExam(examId, exam).then(() => {
                dataCopy = dataCopy.filter(ex => ex.examId != examId)
                setData(dataCopy)
                onPassedAction(exam)
            }
        )
    }

    return (
        <TableContainer style={{margin: "1% 0"}} component={Paper}>
            <Table>
                <TableHead>
                    <TableRow key={'head.row'}>
                        {headers.map((el, elIndex) =>
                            <TableCell key={'head.' + elIndex} align={"center"}
                                       style={{fontWeight: "bolder"}}>{el}</TableCell>)}
                    </TableRow>
                </TableHead>
                <TableBody>
                    {data.map((row, rowIndex) =>
                        <TableRow key={rowIndex + '.row'}>
                            <TableCell align={"center"}><IconButton
                                onClick={() => onPassedClick(row.examId)}><SwapVertIcon/></IconButton></TableCell>
                            {[row.examId, row.student_initials, row.title, row.student_group, row.type, row.classroom,
                                moment(row.dateTime).format("DD.MM.YYYY HH:mm"), row.lecturers.map(l => `${l.lastName} ${l.firstName} ${l.middleName}`).join(", ")]
                                .map((cell, cellIndex) =>
                                    <TableCell key={rowIndex + "." + cellIndex} align={"center"}
                                               style={{maxWidth: "50px"}}>{cell}</TableCell>)}

                            <TableCell><IconButton onClick={() => onDeleteClick(row.examId)}><DeleteIcon/></IconButton></TableCell>
                        </TableRow>)
                    }
                </TableBody>
            </Table>
        </TableContainer>
    )
}