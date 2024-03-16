import {Checkbox, Paper, Table, TableBody, TableCell, TableContainer, TableHead, TableRow} from "@mui/material";
import {Exam} from "@entities/Exam";
import React, {useMemo} from "react";


interface TableProps {
    data: Exam[],
    selected: number[],
    setSelected: React.Dispatch<React.SetStateAction<number[]>>
}


export const ExamDisplayTable = ({data, selected, setSelected}: TableProps) => {
    const headers = useMemo(() => ["", "Id", "Студент", "Дисциплина", "Группа", "Тип", "Место проведения", "Дата", "Преподаватели"], [])
    const onCheckboxClick = (event: React.ChangeEvent<HTMLInputElement>, element: number) => {
        if (event.target.checked) {
            setSelected([element, ...selected])
        } else {
            setSelected(selected.filter(num => num != element))
        }
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
                            {[row.examId, row.student_initials, row.title, row.student_group, row.type, row.classroom,
                                row.dateTime, row.lecturers.map(l => `${l.lastName} ${l.firstName} ${l.middleName}`).join(", ")]
                                .map((cell, cellIndex) =>
                                    <TableCell key={rowIndex + "." + cellIndex} align={"center"}
                                               style={{maxWidth: "50px"}}>{cell}</TableCell>)}
                        </TableRow>)
                    }
                </TableBody>
            </Table>
        </TableContainer>
    )
}