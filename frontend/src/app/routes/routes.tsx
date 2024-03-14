import {createBrowserRouter} from "react-router-dom";
import {ScheduleDisplayPage} from "@pages/lecturer/ScheduleDisplayPage";

export const routes = createBrowserRouter([
    {
        path: "/",
        element: <ScheduleDisplayPage/>
    }
])