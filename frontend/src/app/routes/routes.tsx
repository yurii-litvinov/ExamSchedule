import {createBrowserRouter} from "react-router-dom";
import {ScheduleDisplayPage} from "@pages/ScheduleDisplayPage.tsx"
import {LoginPage} from "@pages/LoginPage.tsx";
import {NotFoundPage} from "@pages/NotFoundPage.tsx";

// Create routes
export const routes = createBrowserRouter([
    {
        path: "/",
        element: <ScheduleDisplayPage/>
    },
    {
        path: "/login",
        element: <LoginPage/>
    },
    {
        path: "*",
        element: <NotFoundPage/>
    }
])