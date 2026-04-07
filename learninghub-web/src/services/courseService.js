import api from './api'

export const courseService = {
    // Courses
    getPublished() { return api.get('/courses/published') },
    getAll() { return api.get('/courses') },
    getMyCourses() { return api.get('/courses/my') },
    getCourseDetail(id) { return api.get(`/courses/${id}`) },
    createCourse(data) { return api.post('/courses', data) },
    updateCourse(id, data) { return api.put(`/courses/${id}`, data) },
    deleteCourse(id) { return api.delete(`/courses/${id}`) },

    // Modules
    getModules(courseId) { return api.get(`/courses/${courseId}/modules`) },
    createModule(courseId, data) { return api.post(`/courses/${courseId}/modules`, data) },
    updateModule(courseId, moduleId, data) { return api.put(`/courses/${courseId}/modules/${moduleId}`, data) },
    deleteModule(courseId, moduleId) { return api.delete(`/courses/${courseId}/modules/${moduleId}`) },

    // Materials
    createMaterial(moduleId, formData) { return api.post(`/modules/${moduleId}/materials`, formData) },
    updateMaterial(moduleId, materialId, formData) { return api.put(`/modules/${moduleId}/materials/${materialId}`, formData) },
    deleteMaterial(moduleId, materialId) { return api.delete(`/modules/${moduleId}/materials/${materialId}`) }
}