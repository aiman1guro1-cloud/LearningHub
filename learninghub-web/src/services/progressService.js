import api from './api'

export const progressService = {
    // Student: mark a material complete
    markComplete(courseId, materialId) {
        return api.post('/progress', { courseId, materialId })
    },

    // Student: unmark a material
    unmarkComplete(courseId, materialId) {
        return api.delete(`/progress/${courseId}/${materialId}`)
    },

    // Student: get completed material IDs for a course
    getMyProgress(courseId) {
        return api.get(`/progress/course/${courseId}`)
    },

    // Student: get progress across all courses
    getAllMyProgress() {
        return api.get('/progress/my')
    },

    // Instructor: get all students' progress for a course
    getStudentProgress(courseId) {
        return api.get(`/progress/course/${courseId}/students`)
    }
}