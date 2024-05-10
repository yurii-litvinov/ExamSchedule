import {Layout} from "@shared/ui/layout/Layout.tsx";
import Button from "@mui/material/Button";
import {ChangeEvent, FormEvent, useState} from "react";
import {login} from "@shared/services/axios.service.ts";
import {TextField} from "@mui/material";
import {LoginResponse} from "@entities/LoginResponse.ts";
import {setAccessToken, setMyId, setRefreshToken} from "@shared/services/localStorage.service.ts"


// Page for login
export function LoginPage() {
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");

    const onChangeLogin = (event: ChangeEvent<HTMLInputElement>) => {
        setEmail(event.target.value)
    }

    const onChangePassword = (event: ChangeEvent<HTMLInputElement>) => {
        setPassword(event.target.value)
    }

    const onSubmit = (event: FormEvent<HTMLFormElement>) => {
        event.preventDefault()
        login(email, password).then(response => {
            const loginResponse: LoginResponse = response.data
            setAccessToken(loginResponse.accessToken)
            setRefreshToken(loginResponse.refreshToken)
            setMyId(loginResponse.staff)
            window.location.assign("/")
        }).catch(e => {
            alert("Не удалось войти")
            console.log(e)
        })
    }

    return (
        <Layout>
            <div className="login-block" style={{display: "flex", justifyContent: "center", alignItems: "center"}}>
                <form onSubmit={onSubmit} style={{
                    display: "flex",
                    flexDirection: "column",
                    justifyContent: "center",
                    alignItems: "center"
                }}>
                    <div className="label-form"
                         style={{
                             display: "flex",
                             alignItems: "center",
                             margin: "10px 0",
                         }}>
                        <h3 style={{marginRight: "10px"}}>Логин:</h3>
                        <TextField style={{width: "300px"}} required onChange={onChangeLogin}/>
                    </div>
                    <div className="label-form"
                         style={{
                             display: "flex",
                             alignItems: "center",
                             margin: "10px 0",
                         }}>
                        <h3 style={{marginRight: "10px"}}>Пароль:</h3>
                        <TextField type={"password"} style={{width: "300px"}} required onChange={onChangePassword}/>
                    </div>
                    <Button variant={"contained"} type={"submit"}>Войти</Button>
                </form>
            </div>
        </Layout>
    );
}