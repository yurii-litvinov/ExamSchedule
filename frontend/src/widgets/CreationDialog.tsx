import {Dialog, Select, SelectChangeEvent, TextField} from "@mui/material";
import Button from "@mui/material/Button";
import {DesktopDatePicker, LocalizationProvider} from "@mui/x-date-pickers";
import {AdapterMoment} from "@mui/x-date-pickers/AdapterMoment";
import MenuItem from "@mui/material/MenuItem";
import {ChangeEvent, FormEvent, useState} from "react";
import {Moment} from "moment/moment";
import {InputExam} from "@entities/Exam.ts";
import {insertExam} from "@shared/services/axios.service.ts";


interface DialogProps {
    open: boolean,
    closeDialog: () => void
}

export const CreationDialog = ({open, closeDialog}: DialogProps) => {
    const [studentInitials, setStudentInitials] = useState("");
    const [studentGroup, setStudentGroup] = useState("");
    const [title, setTitle] = useState("");
    const [lecturerInitials, setLecturerInitials] = useState("");
    const [dateTime, setDateTime] = useState("");
    const [location, setLocation] = useState("");
    const [examType, setExamType] = useState("Пересдача");

    const onChangeStudentInitials = (event: ChangeEvent<HTMLInputElement>) => {
        setStudentInitials(event.target.value)
    }

    const onChangeStudentGroup = (event: ChangeEvent<HTMLInputElement>) => {
        setStudentGroup(event.target.value)
    }

    const onChangeTitle = (event: ChangeEvent<HTMLInputElement>) => {
        setTitle(event.target.value)
    }

    const onChangeLecturerInitials = (event: ChangeEvent<HTMLInputElement>) => {
        setLecturerInitials(event.target.value)
    }

    const onChangeDateTime = (value: Moment | null) => {
        setDateTime(value?.utc().format() ?? "")
    }

    const onChangeLocation = (event: ChangeEvent<HTMLInputElement>) => {
        setLocation(event.target.value)
    }

    const onChangeExamType = (event: SelectChangeEvent) => {
        setExamType(event.target.value)
    }


    const onSubmit = (event: FormEvent<HTMLFormElement>) => {
        event.preventDefault()
        const exam: InputExam = {
            title: title,
            type: examType,
            studentInitials: studentInitials,
            studentGroup: studentGroup,
            classroom: location,
            dateTime: dateTime,
            lecturersInitials: [lecturerInitials]
        }
        insertExam(exam).then(() => alert("Запись успешно добавлена")).catch(e => {
            alert(e)
            console.log(e)
        })
        closeDialog()
    }


    return <Dialog open={open} PaperProps={{sx: {width: "93%", height: "95%", maxWidth: "100%", maxHeight: "100%"}}}>
        <form style={{
            height: "100%",
            display: "flex",
            flexDirection: "column",
            margin: "20px 20px",
            justifyContent: "space-between",
            alignItems: "flex-start"
        }} onSubmit={onSubmit}>
            <div className="student-block" style={{display: "flex", flexDirection: "column"}}>
                <div className="label-form"
                     style={{
                         display: "flex",
                         alignItems: "center",
                         margin: "10px 0",
                         justifyContent: "space-between"
                     }}>
                    <h3 style={{marginRight: "10px"}}>ФИО студента:</h3>
                    <TextField style={{width: "300px"}} required onChange={onChangeStudentInitials}/>
                </div>
                <div className="label-form"
                     style={{
                         display: "flex",
                         alignItems: "center",
                         margin: "10px 0",
                         justifyContent: "space-between"
                     }}>
                    <h3 style={{marginRight: "10px"}}>Группа:</h3>
                    <TextField style={{width: "300px"}} required onChange={onChangeStudentGroup}/>
                </div>
                <div className="label-form"
                     style={{
                         display: "flex",
                         alignItems: "center",
                         margin: "10px 0",
                         justifyContent: "space-between"
                     }}>
                    <h3 style={{marginRight: "10px"}}>Дисциплина:</h3>
                    <TextField style={{width: "300px"}} required onChange={onChangeTitle}/>
                </div>
            </div>

            <div className="lecturer-block" style={{display: "flex", flexDirection: "column"}}>
                <div className="label-form"
                     style={{display: "flex", alignItems: "center", margin: "10px 0"}}>
                    <h3 style={{marginRight: "10px"}}>ФИО преподавателя:</h3>
                    <TextField style={{width: "300px"}} required onChange={onChangeLecturerInitials}/>
                </div>
            </div>

            <div className="type-date-location-block" style={{display: "flex", flexDirection: "column"}}>
                <div className="label-form"
                     style={{display: "flex", alignItems: "center", margin: "10px 0"}}>
                    <h3 style={{marginRight: "10px"}}>Тип экзамена:</h3>
                    <Select required value={examType} onChange={onChangeExamType}>
                        <MenuItem value={"Сдача"}>Сдача</MenuItem>
                        <MenuItem value={"Пересдача"}>Пересдача</MenuItem>
                        <MenuItem value={"Комиссия"}>Комиссия</MenuItem>
                    </Select>
                </div>
                <div className="label-form"
                     style={{display: "flex", alignItems: "center", margin: "10px 0"}}>
                    <h3 style={{marginRight: "10px"}}>Дата и время проведения:</h3>
                    <LocalizationProvider dateAdapter={AdapterMoment}>
                        <DesktopDatePicker onChange={onChangeDateTime}/>
                    </LocalizationProvider>
                </div>
                <div className="label-form"
                     style={{
                         display: "flex",
                         alignItems: "center",
                         margin: "10px 0",
                     }}>
                    <h3 style={{marginRight: "10px"}}>Место проведения:</h3>
                    <TextField style={{width: "300px"}} required onChange={onChangeLocation}/>
                </div>
            </div>
            <div className="buttons" style={{alignSelf: "flex-end"}}>
                <Button style={{margin: "0 10px"}} variant={"outlined"} type={"submit"}>Создать</Button>
                <Button variant={"contained"} onClick={closeDialog}>Закрыть</Button>
            </div>
        </form>
    </Dialog>
}