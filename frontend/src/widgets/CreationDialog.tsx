import {Dialog, Grid, Paper, Select, SelectChangeEvent, TextField} from "@mui/material";
import Button from "@mui/material/Button";
import {DesktopDateTimePicker, LocalizationProvider} from "@mui/x-date-pickers";
import {AdapterMoment} from "@mui/x-date-pickers/AdapterMoment";
import MenuItem from "@mui/material/MenuItem";
import {ChangeEvent, FormEvent, useState} from "react";
import {Moment} from "moment/moment";
import {InputExam} from "@entities/Exam.ts";
import {getEducatorTimetable, getLocationTimetable, insertExam} from "@shared/services/axios.service.ts";
import {EducatorEventsDay} from "@entities/EducatorTimetable.ts";
import styled from "@emotion/styled";
import {ClassroomEventsDay} from "@entities/ClassroomTimetable.ts";
import moment from "moment";


interface DialogProps {
    open: boolean,
    closeDialog: () => void
}

// Dialog for creating new exams
export const CreationDialog = ({open, closeDialog}: DialogProps) => {
    const [studentInitials, setStudentInitials] = useState("");
    const [studentGroup, setStudentGroup] = useState("");
    const [title, setTitle] = useState("");
    const [lecturerInitials, setLecturerInitials] = useState("");
    const [dateTime, setDateTime] = useState(moment().utc().format());
    const [location, setLocation] = useState("");
    const [examType, setExamType] = useState("Пересдача");
    const [educatorTimetable, setEducatorTimetable] = useState<EducatorEventsDay[]>();
    const [locationTimetable, setLocationTimetable] = useState<ClassroomEventsDay[]>();
    const [dayValue, setDayValue] = useState("0");

    const Item = styled(Paper)(() => ({
        backgroundColor: '#fff',
    }));

    const onChangeStudentInitials = (event: ChangeEvent<HTMLInputElement>) => {
        setStudentInitials(event.target.value)
    }

    const onChangeStudentGroup = (event: ChangeEvent<HTMLInputElement>) => {
        setStudentGroup(event.target.value)
    }

    const onChangeTitle = (event: ChangeEvent<HTMLInputElement>) => {
        setTitle(event.target.value)
    }

    // On changing initials searches educator timetable
    const onChangeLecturerInitials = (event: ChangeEvent<HTMLInputElement>) => {
        getEducatorTimetable(event.target.value).then(response => {
            const educatorEventsDays: EducatorEventsDay[] = response.data
            setEducatorTimetable(educatorEventsDays)
        })
        setLecturerInitials(event.target.value)
    }

    // On changing datetime searches classroom timetable
    const onChangeDateTime = (value: Moment | null) => {
        setDateTime(value?.utc().format() ?? "")
        const startDate = moment(dateTime)
        const endDate = startDate.add(7, 'days')
        getLocationTimetable(location, moment(dateTime).format("YYYYMMDDHHmm"), endDate.format("YYYYMMDDHHmm")).then(response => {
            const classroomEventsDays: ClassroomEventsDay[] = response.data
            setLocationTimetable(classroomEventsDays)
        })
    }

    // On changing location searches classroom timetable
    const onChangeLocation = (event: ChangeEvent<HTMLInputElement>) => {
        const startDate = moment(dateTime)
        const endDate = startDate.add(7, 'days')
        getLocationTimetable(event.target.value, moment(dateTime).format("YYYYMMDDHHmm"), endDate.format("YYYYMMDDHHmm")).then(response => {
            const classroomEventsDays: ClassroomEventsDay[] = response.data
            setLocationTimetable(classroomEventsDays)
        })
        setLocation(event.target.value)
    }

    const onChangeExamType = (event: SelectChangeEvent) => {
        setExamType(event.target.value)
    }

    const onDayValueChange = (event: SelectChangeEvent) => {
        setDayValue(event.target.value)
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

            <div className="lecturer-block"
                 style={{width: "100%", display: "flex", justifyContent: "space-between", margin: "20px 0"}}>
                <div className="label-form"
                     style={{display: "flex", alignItems: "center", margin: "10px 0"}}>
                    <h3 style={{marginRight: "10px"}}>ФИО преподавателя:</h3>
                    <TextField style={{width: "300px"}} required onChange={onChangeLecturerInitials}/>
                </div>
                <div className="educator-timetable"
                     style={{alignSelf: "flex-start", flexBasis: "50%", display: "flex", flexDirection: "column"}}>
                    {
                        (educatorTimetable?.length ?? 0) > 0 && educatorTimetable &&
                        <>
                            <Select value={dayValue} onChange={onDayValueChange}>
                                {educatorTimetable.map((eventsDay, index) => <MenuItem
                                    value={index}>{eventsDay.dayString}</MenuItem>)}
                            </Select>
                            <Grid container spacing={2}>
                                {educatorTimetable[+dayValue].dayStudyEvents.map(studyEvent =>
                                    <Grid item width={"100%"}>
                                        <Item>
                                            {studyEvent.subject}<br/>
                                            Время: {studyEvent.timeIntervalString}<br/>
                                            Даты: {studyEvent.dates.join(", ")} <br/>
                                            Группа: {studyEvent.contingentUnitNames.map(element => element.item1).join(", ")}
                                        </Item>
                                    </Grid>)}
                            </Grid>
                        </>
                    }

                </div>
            </div>

            <div className="type-date-location-block"
                 style={{width: "100%", display: "flex", justifyContent: "space-between", margin: "20px 0"}}>
                <div className="form-block">
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
                            <DesktopDateTimePicker ampm={false} onChange={onChangeDateTime}/>
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
                <div className="location-timetable"
                     style={{alignSelf: "flex-start", flexBasis: "50%", display: "flex", flexDirection: "column"}}>
                    {
                        (locationTimetable?.length ?? 0) > 0 && locationTimetable &&
                        <>
                            <Select value={dayValue} onChange={onDayValueChange}>
                                {locationTimetable.map((eventsDay, index) => <MenuItem
                                    value={index}>{eventsDay.dayString}</MenuItem>)}
                            </Select>
                            <Grid container spacing={2}>
                                {locationTimetable[+dayValue].dayStudyEvents.map(studyEvent =>
                                    <Grid item width={"100%"}>
                                        <Item>
                                            {studyEvent.subject}<br/>
                                            Время: {studyEvent.timeIntervalString}<br/>
                                            Даты: {studyEvent.dates.join(", ")} <br/>
                                            Группа: {studyEvent.contingentUnitNames.map(element => element.item1).join(", ")}
                                            <br/>
                                            Преподаватель: {studyEvent.educatorsDisplayText}
                                        </Item>
                                    </Grid>)}
                            </Grid>
                        </>
                    }

                </div>
            </div>
            <div className="buttons" style={{alignSelf: "flex-end"}}>
                <Button style={{margin: "0 10px"}} variant={"outlined"} type={"submit"}>Создать</Button>
                <Button variant={"contained"} onClick={closeDialog}>Закрыть</Button>
            </div>
        </form>
    </Dialog>
}