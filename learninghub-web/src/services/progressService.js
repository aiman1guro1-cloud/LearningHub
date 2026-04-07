import api from './api'

export const progressService = {
    getMyProgress(courseId) { return api.get(`/progress/course/${courseId}`) },
    markComplete(courseId, materialId) {
        return api.post('/progress', { courseId, materialId })
    }
}