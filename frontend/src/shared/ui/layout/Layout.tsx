import Header from "@shared/ui/layout/Header";
import {FunctionComponent, ReactElement} from "react";

export const Layout: FunctionComponent<{ children: ReactElement }> = props => {
    return (
        <div>
            <Header/>
            {props.children}
        </div>
    );
};