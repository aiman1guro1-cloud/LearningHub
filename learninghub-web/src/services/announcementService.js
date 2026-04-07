import api from './api'

export const announcementService = {
    getAll(courseId) { return api.get(`/courses/${courseId}/announcements`) },
    create(courseId, data) { return api.post(`/courses/${courseId}/announcements`, data) },
    update(courseId, id, data) { return api.put(`/courses/${courseId}/announcements/${id}`, data) },
    remove(courseId, id) { return api.delete(`/courses/${courseId}/announcements/${id}`) }
}