import axios from 'axios'

const API_URL = 'https://localhost:7001/api'

export const authService = {
    async login(email, password) {
        const response = await axios.post(`${API_URL}/auth/login`, { email, password })
        return response.data
    },

    async register(fullName, email, password, role) {
        const response = await axios.post(`${API_URL}/auth/register`, {
            fullName,
            email,
            password,
            role
        })
        return response.data
    }
}