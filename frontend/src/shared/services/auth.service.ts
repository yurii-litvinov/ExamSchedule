import {
    getAccessToken,
    getRefreshToken,
    setAccessToken,
    setRefreshToken
} from "@shared/services/localStorage.service.ts";
import {axiosService} from "@shared/services/axios.service.ts";

/// Header with access token for axios requests
export const authHeader = () => {
    const token = getAccessToken();

    if (token) {
        return {Authorization: 'Bearer ' + token, "Content-Type": "application/json"};
    } else {
        return {};
    }
}

/// Refresh expired access token
export const refreshToken = async () => {
    const refresh = getRefreshToken()
    const access = getAccessToken()
    const response = await axiosService
        .post("api/refresh/", {refreshToken: refresh, accessToken: access});
    if (response.data.accessToken) {
        localStorage.setAccessToken(response.data.accessToken)
    }
}

export const logout = () => {
    setAccessToken("")
    setRefreshToken("")
}
