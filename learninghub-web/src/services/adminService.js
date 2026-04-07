import api from './api'

export const adminService = {
  // Stats
  getStats()                          { return api.get('/admin/stats') },

  // Users
  getUsers()                          { return api.get('/admin/users') },
  getUser(id)                         { return api.get(`/admin/users/${id}`) },
  createUser(data)                    { return api.post('/admin/users', data) },
  updateUser(id, data)                { return api.put(`/admin/users/${id}`, data) },
  deleteUser(id)                      { return api.delete(`/admin/users/${id}`) },
  resetPassword(id, newPassword)      { return api.post(`/admin/users/${id}/reset-password`, { newPassword }) },

  // Courses
  getAllCourses()                      { return api.get('/admin/courses') },
  deleteCourse(id)                    { return api.delete(`/admin/courses/${id}`) }
}