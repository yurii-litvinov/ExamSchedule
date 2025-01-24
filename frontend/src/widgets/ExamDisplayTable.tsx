import {Checkbox, Paper, Table, TableBody, TableCell, TableContainer, TableHead, TableRow} from "@mui/material";
import {Exam, InputExam} from "@entities/Exam";
import React, {useMemo} from "react";
import {deleteExam, updateExam} from "@shared/services/axios.service.ts";
import DeleteIcon from '@mui/icons-material/Delete';
import EditIcon from '@mui/icons-material/Edit';
import IconButton from "@mui/material/IconButton";
import SwapVertIcon from '@mui/icons-material/SwapVert';
import moment from 'moment';


interface TableProps {
    data: Exam[],
    setData: React.Dispatch<React.SetStateAction<Exam[]>>,
    onPassedAction: (exam: Exam) => void,
    onEditAction: (id: number) => void,
    selected: number[],
    setSelected: React.Dispatch<React.SetStateAction<number[]>>
}


// Table for exam display
export const ExamDisplayTable = ({data, setData, onPassedAction, onEditAction, selected, setSelected}: TableProps) => {
    moment.locale('ru')
    const headers = useMemo(() => ["", "Id", "Студент", "Дисциплина", "Группа", "Тип", "Место проведения", "Дата", "Преподаватели", "Cдан/не сдан", "", ""], [])
    const onCheckboxClick = (event: React.ChangeEvent<HTMLInputElement>, element: number) => {
        if (event.target.checked) {
            setSelected([element, ...selected])
        } else {
            setSelected(selected.filter(num => num != element))
        }
    }

    const onDeleteClick = (examId: number) => {
        let dataCopy = data;
        deleteExam(examId).then(() => {
            dataCopy = dataCopy.filter(ex => ex.examId != examId)
            setData(dataCopy)
        })
    }

    const onEditClick = (examId: number) => {
        onEditAction(examId)
    }

    // Action on change passed status button click
    const onPassedClick = (examId: number) => {
        let dataCopy = data;
        const exam = dataCopy.find(ex => ex.examId == examId)
        if (!exam) return
        const inputExam: InputExam = {
            title: exam.title,
            type: exam.type,
            studentInitials: exam.studentInitials,
            studentGroup: exam.studentGroup.title,
            classroom: exam.classroom,
            dateTime: exam.dateTime,
            lecturersInitials: exam.lecturers.map(lecturer => `${lecturer.lastName} ${lecturer.firstName} ${lecturer.middleName}`)
        }
        exam.isPassed = !exam.isPassed
        updateExam(examId, inputExam).then(() => {
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
                            <TableCell key={rowIndex + ".box"} align={"center"}>
                                <Checkbox onChange={event => onCheckboxClick(event, row.examId)}/>
                            </TableCell>
                            {[row.examId, row.studentInitials, row.title, row.studentGroup.title, row.type, row.classroom,
                                moment(row.dateTime).format("DD.MM.YYYY HH:mm"), row.lecturers.map(l => `${l.lastName} ${l.firstName} ${l.middleName}`).join(", ")]
                                .map((cell, cellIndex) =>
                                    <TableCell key={rowIndex + "." + cellIndex} align={"center"}
                                               style={{maxWidth: "50px"}}>{cell}</TableCell>)}

                            <TableCell align={"center"}><IconButton
                                onClick={() => onPassedClick(row.examId)}><SwapVertIcon/></IconButton></TableCell>
                            <TableCell><IconButton className="edit-button"
                                                   onClick={() => onEditClick(row.examId)}><EditIcon/></IconButton></TableCell>
                            <TableCell><IconButton className="delete-button"
                                                   onClick={() => onDeleteClick(row.examId)}><DeleteIcon/></IconButton></TableCell>
                        </TableRow>)
                    }
                </TableBody>
            </Table>
        </TableContainer>
    )
}