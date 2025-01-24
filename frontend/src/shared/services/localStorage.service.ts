import {Staff} from "@entities/LoginResponse.ts";

export const setAccessToken = (token: string) => localStorage.setItem("accessToken", token)

export const getAccessToken = () => localStorage.getItem("accessToken")

export const setRefreshToken = (token: string) => localStorage.setItem("refreshToken", token)

export const getRefreshToken = () => localStorage.getItem("refreshToken")

export const setMyId = (me: Staff) => localStorage.setItem("me", (me.staffId || -1).toString())

export const getMyId = () => localStorage.getItem("me")