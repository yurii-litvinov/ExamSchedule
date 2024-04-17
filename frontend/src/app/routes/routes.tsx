import {createBrowserRouter} from "react-router-dom";
import {ScheduleDisplayPage} from "@pages/ScheduleDisplayPage.tsx"

export const routes = createBrowserRouter([
    {
        path: "/create",
        element: <ScheduleDisplayPage forEmployee={false}/>
    },
    {
        path: "/",
        element: <ScheduleDisplayPage forEmployee={true}/>
    }
])