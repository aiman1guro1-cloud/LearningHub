import api from './api'

export const enrollmentService = {
    enroll(courseId) { return api.post('/enrollments', { courseId }) },
    unenroll(courseId) { return api.delete(`/enrollments/${courseId}`) },
    getMyEnrollments() { return api.get('/enrollments/my') },
    getCourseEnrollments(courseId) { return api.get(`/enrollments/course/${courseId}`) },
    checkEnrollment(courseId) { return api.get(`/enrollments/check/${courseId}`) }
}