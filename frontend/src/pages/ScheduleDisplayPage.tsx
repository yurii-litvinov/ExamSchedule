import {Layout} from "@shared/ui/layout/Layout.tsx";
import {ExamDisplayTable} from "@widgets/ExamDisplayTable.tsx";
import Button from "@mui/material/Button";
import {ChangeEvent, useEffect, useState} from "react";
import {getExams, getRoleById} from "@shared/services/axios.service.ts";
import {Exam} from "@entities/Exam.ts";
import {Autocomplete, Chip, Collapse, TextField} from "@mui/material";
import {ExpandLess, ExpandMore} from "@mui/icons-material";
import moment from "moment/moment";
import {CreationDialog} from "@widgets/CreationDialog.tsx";
import {getMyId} from "@shared/services/localStorage.service.ts";


// Page for schedule display
export function ScheduleDisplayPage() {
    /// If true, create new exam button will be available
    const [forEmployee, setForEmployee] = useState(false);

    const [filterTags, setFilterTags] = useState<string[]>([])
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

    // Search filter function
    const searchFilter = (exams: Exam[]) => {
        return exams.filter((exam) => {
            const rowData = [exam.examId, exam.student_initials, exam.title, exam.student_group, exam.type, exam.classroom,
                moment(exam.dateTime).format("DD.MM.YYYY HH:mm"), exam.lecturers.map(l => `${l.lastName} ${l.firstName} ${l.middleName}`).join(", ")]
            const expandedFilterTags = searchString ? filterTags.concat(searchString) : filterTags
            return expandedFilterTags.every((tag) => rowData.some((cell) => cell.toString().toLowerCase().includes(tag.toLowerCase())))
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

    const onCloseDialog = () => {
        setOpenDialog(false)
    }

    const onAddTag = (_: React.SyntheticEvent, tags: string[]) => {
        setFilterTags(tags)
        setSearchString("")
    }


    useEffect(() => {
        getRoleById(Number(getMyId())).then(roleResponse => {
            setForEmployee(["Админ", "Сотрудник"].includes(roleResponse.data))
        })
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
                    <Autocomplete
                        multiple
                        id="tags-filled"
                        options={[]}
                        freeSolo
                        onChange={onAddTag}
                        renderTags={(value, getTagProps) =>
                            value.map((option, index) => (
                                <Chip
                                    variant="outlined"
                                    label={option}
                                    {...getTagProps({index})}
                                />
                            ))
                        }
                        renderInput={(params) => (
                            <TextField
                                {...params}
                                className="filter-input" onChange={onSearchChange} value={searchString}
                                placeholder="Поиск по ФИО, дисциплине, группе, типу экзамена"
                            />
                        )}
                    />
                </div>
                <CreationDialog open={openDialog} closeDialog={onCloseDialog}/>
                <div className="table-upper"
                     style={{display: "flex", justifyContent: "space-between", alignItems: "center"}}>
                    <h1>Расписание сдач</h1>
                    {forEmployee &&
                        <div className="table-actions"
                             style={{display: "flex", flexDirection: "column", justifyContent: "space-around"}}>
                            <Button className="create-exam-button" variant={"contained"}
                                    onClick={onOpenDialog}>Добавить</Button>
                        </div>
                    }
                </div>
                <ExamDisplayTable data={searchFilter(tableData)} setData={setTableData} onPassedAction={addToPassed}/>
                <Button className="show-passed-button" color={"inherit"} onClick={handleClick}>
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