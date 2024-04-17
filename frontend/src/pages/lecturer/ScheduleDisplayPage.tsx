import {Layout} from "@shared/ui/layout/Layout";
import {ExamDisplayTable} from "@widgets/ExamDisplayTable";
import Button from "@mui/material/Button";
import {useEffect, useState} from "react";
import {deleteExam, getExams, updateExam} from "@shared/services/axios.service";
import {Exam} from "@entities/Exam";
import {Collapse} from "@mui/material";
import {ExpandLess, ExpandMore} from "@mui/icons-material";


export function ScheduleDisplayPage() {
    const [tableData, setTableData] = useState<Exam[]>([])
    const [passedData, setPassedData] = useState<Exam[]>([])
    const [selected, setSelected] = useState<number[]>([])
    const [open, setOpen] = useState(false)

    const handleClick = () => {
        setOpen(!open)
    }

    const onDeleteClick = () => {
        let dataCopy = tableData;
        let passedCopy = passedData;
        for (const examId of selected) {
            deleteExam(examId).then(() => {
                dataCopy = dataCopy.filter(ex => ex.examId != examId)
                passedCopy = passedCopy.filter(ex => ex.examId != examId)
                setTableData(dataCopy)
                setPassedData(passedCopy)
            })
        }
    }

    const onPassedClick = () => {
        let dataCopy = tableData;
        const passedCopy = passedData;
        for (const examId of selected) {
            const exam = dataCopy.find(ex => ex.examId == examId)
            if (!exam) continue
            exam.isPassed = true
            updateExam(examId, exam).then(() => {
                    dataCopy = dataCopy.filter(ex => ex.examId != examId)
                    passedCopy.push(exam)
                    setTableData(dataCopy)
                    setPassedData(passedCopy)
                }
            )
        }
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
            <div className="content" style={{margin: "5% 10%"}}>
                <div className="table-upper"
                     style={{display: "flex", justifyContent: "space-between", alignItems: "center"}}>
                    <h1>Расписание сдач</h1>
                    <div className="table-actions"
                         style={{display: "flex", flexDirection: "column", justifyContent: "space-around"}}>
                        <Button variant='outlined' onClick={onPassedClick}>Отметить как сдано</Button>
                        <Button variant='contained' onClick={onDeleteClick}>Удалить</Button>
                    </div>
                </div>
                <ExamDisplayTable data={tableData} selected={selected} setSelected={setSelected}/>
                <Button color={"inherit"} onClick={handleClick}>
                    <h1>Сдано</h1>
                    {open ? <ExpandLess/> : <ExpandMore/>}
                </Button>
                <Collapse in={open} timeout="auto" unmountOnExit>
                    <ExamDisplayTable data={passedData} selected={selected} setSelected={setSelected}/>
                </Collapse>
            </div>
        </Layout>
    );
}