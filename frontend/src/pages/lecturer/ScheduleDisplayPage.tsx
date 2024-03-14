import {Layout} from "@shared/ui/layout/Layout";
import {ExamDisplayTable} from "@widgets/ExamDisplayTable";
import Button from "@mui/material/Button";
import {useEffect, useState} from "react";
import {deleteExam, getExams} from "@shared/services/axios.service";
import {Exam} from "@entities/Exam";


export function ScheduleDisplayPage() {
    const [tableData, setTableData] = useState<Exam[]>([])
    const [selected, setSelected] = useState<number[]>([])

    const onDeleteClick = () => {
        for (const ind of selected) {
            deleteExam(tableData[ind].examId).then(() => {
            })
        }
    }


    useEffect(() => {
        getExams().then(r => {
            const responseData: Exam[] = r.data
            setTableData(responseData)
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
                        <Button variant='outlined' onClick={onDeleteClick}>Отметить как сдано</Button>
                        <Button variant='contained' onClick={onDeleteClick}>Удалить</Button>
                    </div>
                </div>
                <ExamDisplayTable data={tableData} selected={selected} setSelected={setSelected}/>
            </div>
        </Layout>
    );
}