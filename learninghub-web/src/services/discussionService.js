import api from './api'

export const discussionService = {
    getPosts(courseId) { return api.get(`/courses/${courseId}/discussions`) },
    getPost(courseId, postId) { return api.get(`/courses/${courseId}/discussions/${postId}`) },
    createPost(courseId, data) { return api.post(`/courses/${courseId}/discussions`, data) },
    updatePost(courseId, postId, data) { return api.put(`/courses/${courseId}/discussions/${postId}`, data) },
    deletePost(courseId, postId) { return api.delete(`/courses/${courseId}/discussions/${postId}`) },
    addComment(courseId, postId, data) { return api.post(`/courses/${courseId}/discussions/${postId}/comments`, data) },
    deleteComment(courseId, postId, commentId) { return api.delete(`/courses/${courseId}/discussions/${postId}/comments/${commentId}`) }
}