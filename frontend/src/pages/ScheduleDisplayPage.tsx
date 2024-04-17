import {Layout} from "@shared/ui/layout/Layout.tsx";
import {ExamDisplayTable} from "@widgets/ExamDisplayTable.tsx";
import Button from "@mui/material/Button";
import {ChangeEvent, useEffect, useState} from "react";
import {getExams} from "@shared/services/axios.service.ts";
import {Exam} from "@entities/Exam.ts";
import {Collapse, TextField} from "@mui/material";
import {ExpandLess, ExpandMore} from "@mui/icons-material";
import moment from "moment/moment";
import {CreationDialog} from "@widgets/CreationDialog.tsx";


interface ScheduleDisplayProps {
    forEmployee: boolean,
}

export function ScheduleDisplayPage({forEmployee = false}: ScheduleDisplayProps) {
    const [tableData, setTableData] = useState<Exam[]>([])
    const [passedData, setPassedData] = useState<Exam[]>([])
    const [openPassed, setOpenPassed] = useState(false)
    const [openDialog, setOpenDialog] = useState(false)
    const [searchString, setSearchString] = useState("")

    const handleClick = () => {
        setOpenPassed(!openPassed)
    }

    const onSearchChange = (event: ChangeEvent<HTMLInputElement>) => {
        setSearchString(event.target.value)
    }

    const searchFilter = (exams: Exam[]) => {
        if (!searchString) return exams
        return exams.filter((exam) => {
            const rowData = [exam.examId, exam.student_initials, exam.title, exam.student_group, exam.type, exam.classroom,
                moment(exam.dateTime).format("DD.MM.YYYY HH:mm"), exam.lecturers.map(l => `${l.lastName} ${l.firstName} ${l.middleName}`).join(", ")]
            return rowData.some((cell) => cell.toString().includes(searchString))
        })
    }

    const addToPassed = (exam: Exam) => {
        const passedCopy = passedData;
        passedCopy.push(exam)
        setPassedData(passedCopy)
    }

    const addToUnpassed = (exam: Exam) => {
        const dataCopy = tableData;
        dataCopy.push(exam)
        setTableData(dataCopy)
    }

    const onOpenDialog = () => {
        setOpenDialog(true)
    }


    useEffect(() => {
        getExams().then(r => {
            const responseData: Exam[] = r.data
            setTableData(responseData.filter(ex => !ex.isPassed))
            setPassedData(responseData.filter(ex => ex.isPassed))
        })
    }, [])

    return (
        <Layout>
            <div className="content" style={{margin: "2% 10%"}}>
                <div className="search" style={{marginBottom: "20px"}}>
                    <TextField onChange={onSearchChange} value={searchString}
                               placeholder={"Поиск по ФИО, дисциплине, группе, типу экзамена"} fullWidth/>
                </div>
                <CreationDialog open={openDialog} closeDialog={() => setOpenDialog(false)}/>
                <div className="table-upper"
                     style={{display: "flex", justifyContent: "space-between", alignItems: "center"}}>
                    <h1>Расписание сдач</h1>
                    {forEmployee &&
                        <div className="table-actions"
                             style={{display: "flex", flexDirection: "column", justifyContent: "space-around"}}>
                            <Button variant={"contained"} onClick={onOpenDialog}>Добавить</Button>
                        </div>
                    }
                </div>
                <ExamDisplayTable data={searchFilter(tableData)} setData={setTableData} onPassedAction={addToPassed}/>
                <Button color={"inherit"} onClick={handleClick}>
                    <h1>Сдано</h1>
                    {openPassed ? <ExpandLess/> : <ExpandMore/>}
                </Button>
                <Collapse in={openPassed} timeout="auto" unmountOnExit>
                    <ExamDisplayTable data={searchFilter(passedData)} setData={setPassedData}
                                      onPassedAction={addToUnpassed}/>
                </Collapse>
            </div>
        </Layout>
    );
}