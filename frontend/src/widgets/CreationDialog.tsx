import {Dialog, Grid, Paper, Select, SelectChangeEvent, TextField} from "@mui/material";
import Button from "@mui/material/Button";
import {DesktopDateTimePicker, LocalizationProvider} from "@mui/x-date-pickers";
import {AdapterMoment} from "@mui/x-date-pickers/AdapterMoment";
import MenuItem from "@mui/material/MenuItem";
import {ChangeEvent, FormEvent, useEffect, useState} from "react";
import {Moment} from "moment/moment";
import {Exam, InputExam} from "@entities/Exam.ts";
import {
    getEducatorTimetable, getExam, getGroupTimetable,
    getLocationTimetable,
    insertExam,
    updateExam
} from "@shared/services/axios.service.ts";
import {EducatorEventsDay} from "@entities/EducatorTimetable.ts";
import styled from "@emotion/styled";
import {ClassroomEventsDay} from "@entities/ClassroomTimetable.ts";
import moment from "moment";
import {GroupEventsDay} from "@entities/GroupTimetable.ts";


interface DialogProps {
    open: boolean,
    closeDialog: () => void,
    editExamId: number,
}

// Dialog for creating new exams
export const CreationDialog = ({open, closeDialog, editExamId = -1}: DialogProps) => {
    const [editExam, setEditExam] = useState<Exam>();
    const [studentInitials, setStudentInitials] = useState(editExam?.studentInitials ?? "");
    const [studentGroup, setStudentGroup] = useState(editExam?.studentGroup ?? "");
    const [title, setTitle] = useState(editExam?.title ?? "");
    const [lecturerInitials, setLecturerInitials] = useState("");
    const [dateTime, setDateTime] = useState(moment(editExam?.dateTime).utc().format() ?? moment().utc().format());
    const [location, setLocation] = useState(editExam?.classroom ?? "");
    const [examType, setExamType] = useState(editExam?.type ?? "");
    const [educatorTimetable, setEducatorTimetable] = useState<EducatorEventsDay[]>();
    const [locationTimetable, setLocationTimetable] = useState<ClassroomEventsDay[]>();
    const [groupTimetable, setGroupTimetable] = useState<GroupEventsDay[]>();
    const [dayValue, setDayValue] = useState("0");

    const Item = styled(Paper)(() => ({
        backgroundColor: '#fff',
    }));

    const onChangeStudentInitials = (event: ChangeEvent<HTMLInputElement>) => {
        setStudentInitials(event.target.value)
    }

    const onChangeStudentGroup = (event: ChangeEvent<HTMLInputElement>) => {
        const startDate = moment(dateTime)
        getGroupTimetable(event.target.value, startDate.format("YYYY-MM-DD")).then(response => {
            const groupEventsDays: GroupEventsDay[] = response.data
            setGroupTimetable(groupEventsDays)
        })
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
        const endDate = moment(value?.utc().format() ?? "").add(7, 'days')
        getLocationTimetable(location, moment(value?.utc().format() ?? "").format("YYYYMMDDHHmm"), endDate.format("YYYYMMDDHHmm")).then(response => {
            const classroomEventsDays: ClassroomEventsDay[] = response.data
            setLocationTimetable(classroomEventsDays)
        })
        getGroupTimetable(studentGroup, moment(value?.utc().format() ?? "").format("YYYY-MM-DD")).then(response => {
            const groupEventsDays: GroupEventsDay[] = response.data
            setGroupTimetable(groupEventsDays)
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
        if (editExamId >= 0) {
            updateExam(editExamId, exam).then(() => alert("Запись успешно изменена")).catch(e => {
                alert(e)
                console.log(e)
            })
        } else {
            insertExam(exam).then(() => alert("Запись успешно добавлена")).catch(e => {
                alert(e)
                console.log(e)
            })
        }
        closeDialog()
    }


    useEffect(() => {
        getExam(editExamId).then(r => {
            const responseData: Exam = r.data[0]
            setEditExam(responseData)
            setStudentInitials(responseData?.studentInitials ?? "");
            setStudentGroup(responseData?.studentGroup ?? "");
            setTitle(responseData?.title ?? "");
            setDateTime(moment(responseData?.dateTime).utc().format() ?? moment().utc().format());
            setLocation(responseData?.classroom ?? "");
            setExamType(responseData?.type ?? "");
        })
    }, [editExamId])


    return <Dialog open={open} PaperProps={{sx: {width: "93%", height: "95%", maxWidth: "100%", maxHeight: "100%"}}}>
        <form style={{
            height: "100%",
            display: "flex",
            flexDirection: "column",
            margin: "20px 20px",
            justifyContent: "space-between",
            alignItems: "flex-start"
        }} onSubmit={onSubmit}>
            <div className="student-block"
                 style={{width: "100%", display: "flex", justifyContent: "space-between", margin: "20px 0"}}>
                <div className="student-forms" style={{display: "flex", flexDirection: "column"}}>
                    <div className="label-form"
                         style={{
                             display: "flex",
                             alignItems: "center",
                             margin: "10px 0",
                             justifyContent: "space-between"
                         }}>
                        <h3 style={{marginRight: "10px"}}>ФИО студента:</h3>
                        <TextField className="student-initials-input" style={{width: "300px"}} required
                                   onChange={onChangeStudentInitials} value={studentInitials}/>
                    </div>
                    <div className="label-form"
                         style={{
                             display: "flex",
                             alignItems: "center",
                             margin: "10px 0",
                             justifyContent: "space-between"
                         }}>
                        <h3 style={{marginRight: "10px"}}>Группа:</h3>
                        <TextField className="student-group-input" style={{width: "300px"}} required
                                   onChange={onChangeStudentGroup} value={studentGroup}/>
                    </div>
                    <div className="label-form"
                         style={{
                             display: "flex",
                             alignItems: "center",
                             margin: "10px 0",
                             justifyContent: "space-between"
                         }}>
                        <h3 style={{marginRight: "10px"}}>Дисциплина:</h3>
                        <TextField className="exam-title-input" style={{width: "300px"}} required
                                   onChange={onChangeTitle}
                                   value={title}/>
                    </div>
                </div>

                <div className="group-timetable"
                     style={{alignSelf: "flex-start", flexBasis: "50%", display: "flex", flexDirection: "column"}}>
                    {
                        (groupTimetable?.length ?? 0) > 0 && groupTimetable &&
                        <>
                            <Select value={dayValue} onChange={onDayValueChange}>
                                {groupTimetable.map((eventsDay, index) => <MenuItem
                                    value={index}>{eventsDay.dayString}</MenuItem>)}
                            </Select>
                            <Grid container spacing={2}>
                                {groupTimetable[+dayValue].dayStudyEvents.map(studyEvent =>
                                    <Grid item width={"100%"}>
                                        <Item>
                                            {studyEvent.subject}<br/>
                                            Время: {studyEvent.timeIntervalString}<br/>
                                            Даты: {studyEvent.locationsDisplayText} <br/>
                                            Преподаватель: {studyEvent.educatorsDisplayText}
                                        </Item>
                                    </Grid>)}
                            </Grid>
                        </>
                    }

                </div>
            </div>

            <div className="lecturer-block"
                 style={{width: "100%", display: "flex", justifyContent: "space-between", margin: "20px 0"}}>
                <div className="label-form"
                     style={{display: "flex", alignItems: "center", margin: "10px 0"}}>
                    <h3 style={{marginRight: "10px"}}>ФИО преподавателя:</h3>
                    <TextField className="lecturer-initials-input" style={{width: "300px"}} required
                               onChange={onChangeLecturerInitials} value={lecturerInitials}/>
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
                        <Select className="exam-type-input" required value={examType} onChange={onChangeExamType}>
                            <MenuItem value={"Сдача"}>Сдача</MenuItem>
                            <MenuItem value={"Пересдача"}>Пересдача</MenuItem>
                            <MenuItem value={"Комиссия"}>Комиссия</MenuItem>
                        </Select>
                    </div>
                    <div className="label-form"
                         style={{display: "flex", alignItems: "center", margin: "10px 0"}}>
                        <h3 style={{marginRight: "10px"}}>Дата и время проведения:</h3>
                        <LocalizationProvider dateAdapter={AdapterMoment}>
                            <DesktopDateTimePicker className="dadatetime-input" ampm={false}
                                                   onChange={onChangeDateTime} value={moment(dateTime)}
                                                   format={"DD.MM.YYYY HH:mm"}/>
                        </LocalizationProvider>
                    </div>
                    <div className="label-form"
                         style={{
                             display: "flex",
                             alignItems: "center",
                             margin: "10px 0",
                         }}>
                        <h3 style={{marginRight: "10px"}}>Место проведения:</h3>
                        <TextField className="location-input" style={{width: "300px"}} required
                                   onChange={onChangeLocation} value={location}/>
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
                <Button className="submit-button" style={{margin: "0 10px"}} variant={"contained"}
                        type={"submit"}>Сохранить</Button>
                <Button className="close-button" variant={"outlined"} onClick={closeDialog}>Закрыть</Button>
            </div>
        </form>
    </Dialog>
}