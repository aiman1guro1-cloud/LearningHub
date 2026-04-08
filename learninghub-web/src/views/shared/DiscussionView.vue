<template>
    <div class="min-h-screen bg-gray-50">

        <!-- Navbar -->
        <nav class="bg-white border-b border-gray-200 px-6 py-4 flex items-center justify-between">
            <div class="flex items-center gap-3">
                <button @click="goBack"
                        class="text-gray-400 hover:text-gray-600 transition-colors">
                    ← Back
                </button>
                <span class="text-gray-300">|</span>
                <span class="font-semibold text-gray-800 truncate max-w-xs">{{ courseName }}</span>
            </div>
            <span class="text-sm font-medium text-gray-500">Discussion Forum</span>
        </nav>

        <div class="max-w-3xl mx-auto px-6 py-8">

            <!-- Header -->
            <div class="flex items-center justify-between mb-6">
                <div>
                    <h1 class="text-2xl font-bold text-gray-800">Discussion Forum</h1>
                    <p class="text-gray-500 text-sm mt-1">Ask questions and share ideas</p>
                </div>
                <button @click="openCreateModal"
                        class="bg-primary-600 hover:bg-primary-700 text-white px-4 py-2 rounded-lg text-sm font-medium transition-colors">
                    + New Post
                </button>
            </div>

            <!-- Loading -->
            <div v-if="loading" class="text-center py-16 text-gray-400">Loading discussions...</div>

            <!-- Empty -->
            <div v-else-if="posts.length === 0" class="text-center py-16">
                <div class="text-5xl mb-4">💬</div>
                <p class="text-gray-500">No posts yet. Start the conversation!</p>
            </div>

            <!-- Post List -->
            <div v-else class="space-y-3">
                <div v-for="post in posts" :key="post.id"
                     class="bg-white rounded-xl border border-gray-200 p-5 hover:shadow-sm transition-shadow cursor-pointer"
                     @click="openPost(post)">
                    <div class="flex items-start justify-between gap-4">
                        <div class="flex-1 min-w-0">
                            <h3 class="font-semibold text-gray-800 mb-1 hover:text-primary-600 transition-colors">
                                {{ post.title }}
                            </h3>
                            <p class="text-sm text-gray-500 line-clamp-2 mb-3">{{ post.content }}</p>
                            <div class="flex items-center gap-3 text-xs text-gray-400">
                                <span class="flex items-center gap-1">
                                    <span :class="{
                    'bg-blue-100 text-blue-700':   post.authorRole === 'Instructor',
                    'bg-purple-100 text-purple-700': post.authorRole === 'Admin',
                    'bg-gray-100 text-gray-600':    post.authorRole === 'Student'
                  }" class="px-1.5 py-0.5 rounded text-xs font-medium">
                                        {{ post.authorRole }}
                                    </span>
                                    {{ post.authorName }}
                                </span>
                                <span>·</span>
                                <span>{{ formatDate(post.createdAt) }}</span>
                                <span>·</span>
                                <span>💬 {{ post.commentCount }} {{ post.commentCount === 1 ? 'reply' : 'replies' }}</span>
                            </div>
                        </div>

                        <!-- Delete button (own posts or instructor/admin) -->
                        <button v-if="canDelete(post)"
                                @click.stop="confirmDeletePost(post)"
                                class="shrink-0 text-xs text-red-400 hover:text-red-600 px-2 py-1 rounded hover:bg-red-50">
                            Delete
                        </button>
                    </div>
                </div>
            </div>
        </div>

        <!-- ── Post Detail Modal ────────────────────────────── -->
        <div v-if="activePost"
             class="fixed inset-0 bg-black bg-opacity-40 flex items-center justify-center z-50 p-4">
            <div class="bg-white rounded-2xl shadow-xl w-full max-w-2xl max-h-[90vh] flex flex-col">

                <!-- Modal Header -->
                <div class="px-6 pt-6 pb-4 border-b border-gray-100">
                    <div class="flex items-start justify-between gap-4">
                        <div>
                            <h2 class="text-lg font-bold text-gray-800 mb-1">{{ activePost.title }}</h2>
                            <div class="flex items-center gap-2 text-xs text-gray-400">
                                <span :class="{
                  'bg-blue-100 text-blue-700':     activePost.authorRole === 'Instructor',
                  'bg-purple-100 text-purple-700': activePost.authorRole === 'Admin',
                  'bg-gray-100 text-gray-600':     activePost.authorRole === 'Student'
                }" class="px-1.5 py-0.5 rounded font-medium">
                                    {{ activePost.authorRole }}
                                </span>
                                <span>{{ activePost.authorName }}</span>
                                <span>·</span>
                                <span>{{ formatDate(activePost.createdAt) }}</span>
                            </div>
                        </div>
                        <button @click="activePost = null"
                                class="text-gray-400 hover:text-gray-600 text-xl shrink-0">
                            ✕
                        </button>
                    </div>
                    <p class="text-sm text-gray-600 mt-3 leading-relaxed">{{ activePost.content }}</p>
                </div>

                <!-- Comments -->
                <div class="flex-1 overflow-y-auto px-6 py-4 space-y-3">
                    <p class="text-xs font-semibold text-gray-400 uppercase tracking-wide mb-3">
                        {{ activePost.comments.length }} {{ activePost.comments.length === 1 ? 'Reply' : 'Replies' }}
                    </p>

                    <div v-if="activePost.comments.length === 0"
                         class="text-center py-6 text-gray-400 text-sm">
                        No replies yet. Be the first to reply!
                    </div>

                    <div v-for="comment in activePost.comments" :key="comment.id"
                         class="flex gap-3">
                        <!-- Avatar -->
                        <div class="w-8 h-8 rounded-full flex items-center justify-center text-xs font-bold shrink-0"
                             :class="{
                'bg-blue-100 text-blue-700':     comment.authorRole === 'Instructor',
                'bg-purple-100 text-purple-700': comment.authorRole === 'Admin',
                'bg-gray-100 text-gray-600':     comment.authorRole === 'Student'
              }">
                            {{ comment.authorName.charAt(0).toUpperCase() }}
                        </div>

                        <div class="flex-1 bg-gray-50 rounded-xl px-4 py-3">
                            <div class="flex items-center justify-between mb-1">
                                <div class="flex items-center gap-2">
                                    <span class="text-sm font-semibold text-gray-700">{{ comment.authorName }}</span>
                                    <span :class="{
                    'bg-blue-100 text-blue-700':     comment.authorRole === 'Instructor',
                    'bg-purple-100 text-purple-700': comment.authorRole === 'Admin',
                    'bg-gray-100 text-gray-600':     comment.authorRole === 'Student'
                  }" class="text-xs px-1.5 py-0.5 rounded font-medium">
                                        {{ comment.authorRole }}
                                    </span>
                                </div>
                                <div class="flex items-center gap-2">
                                    <span class="text-xs text-gray-400">{{ formatDate(comment.createdAt) }}</span>
                                    <button v-if="canDeleteComment(comment)"
                                            @click="deleteComment(comment)"
                                            class="text-xs text-red-400 hover:text-red-600">
                                        Delete
                                    </button>
                                </div>
                            </div>
                            <p class="text-sm text-gray-600">{{ comment.content }}</p>
                        </div>
                    </div>
                </div>

                <!-- Add Comment -->
                <div class="px-6 py-4 border-t border-gray-100">
                    <div class="flex gap-3">
                        <div class="w-8 h-8 rounded-full bg-primary-100 flex items-center justify-center text-xs font-bold text-primary-700 shrink-0">
                            {{ user?.fullName?.charAt(0)?.toUpperCase() }}
                        </div>
                        <div class="flex-1 flex gap-2">
                            <input v-model="newComment" type="text"
                                   placeholder="Write a reply..."
                                   @keyup.enter="submitComment"
                                   class="flex-1 border border-gray-300 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-primary-500" />
                            <button @click="submitComment" :disabled="!newComment.trim() || submittingComment"
                                    class="bg-primary-600 hover:bg-primary-700 text-white px-4 py-2 rounded-lg text-sm font-medium disabled:opacity-50 transition-colors">
                                {{ submittingComment ? '...' : 'Reply' }}
                            </button>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <!-- Create Post Modal -->
        <div v-if="showCreateModal"
             class="fixed inset-0 bg-black bg-opacity-40 flex items-center justify-center z-50 p-4">
            <div class="bg-white rounded-2xl shadow-xl w-full max-w-lg p-6">
                <h2 class="text-lg font-bold text-gray-800 mb-4">New Discussion Post</h2>

                <div v-if="createError" class="bg-red-50 text-red-600 text-sm rounded-lg px-4 py-2 mb-4">
                    {{ createError }}
                </div>

                <div class="space-y-4">
                    <div>
                        <label class="block text-sm font-medium text-gray-700 mb-1">Title</label>
                        <input v-model="createForm.title" type="text" placeholder="What's your question or topic?"
                               class="w-full border border-gray-300 rounded-lg px-4 py-2.5 text-sm focus:outline-none focus:ring-2 focus:ring-primary-500" />
                    </div>
                    <div>
                        <label class="block text-sm font-medium text-gray-700 mb-1">Content</label>
                        <textarea v-model="createForm.content" rows="4"
                                  placeholder="Describe your question or idea in detail..."
                                  class="w-full border border-gray-300 rounded-lg px-4 py-2.5 text-sm focus:outline-none focus:ring-2 focus:ring-primary-500 resize-none" />
                    </div>
                </div>

                <div class="flex justify-end gap-3 mt-6">
                    <button @click="showCreateModal = false"
                            class="px-4 py-2 text-sm text-gray-600 font-medium hover:text-gray-800">
                        Cancel
                    </button>
                    <button @click="submitPost" :disabled="creating"
                            class="px-4 py-2 bg-primary-600 hover:bg-primary-700 text-white text-sm font-medium rounded-lg disabled:opacity-60 transition-colors">
                        {{ creating ? 'Posting...' : 'Post' }}
                    </button>
                </div>
            </div>
        </div>

        <!-- Delete Post Confirm -->
        <div v-if="showDeletePostModal"
             class="fixed inset-0 bg-black bg-opacity-40 flex items-center justify-center z-50 p-4">
            <div class="bg-white rounded-2xl shadow-xl w-full max-w-sm p-6 text-center">
                <div class="text-4xl mb-3">🗑️</div>
                <h2 class="text-lg font-bold text-gray-800 mb-2">Delete Post?</h2>
                <p class="text-sm text-gray-500 mb-6">This will also delete all replies.</p>
                <div class="flex gap-3 justify-center">
                    <button @click="showDeletePostModal = false"
                            class="px-4 py-2 text-sm border border-gray-300 rounded-lg hover:bg-gray-50">
                        Cancel
                    </button>
                    <button @click="executeDeletePost" :disabled="deleting"
                            class="px-4 py-2 bg-red-500 text-white text-sm rounded-lg disabled:opacity-60 hover:bg-red-600">
                        {{ deleting ? 'Deleting...' : 'Delete' }}
                    </button>
                </div>
            </div>
        </div>

    </div>
</template>

<script setup>
    import { ref, onMounted } from 'vue'
    import { useRouter, useRoute } from 'vue-router'
    import { useAuthStore } from '../../stores/authStore'
    import { useToastStore } from '../../stores/toastStore'
    import { discussionService } from '../../services/discussionService'
    import { courseService } from '../../services/courseService'

    const router = useRouter()
    const route = useRoute()
    const authStore = useAuthStore()
    const toast = useToastStore()
    const user = authStore.user
    const courseId = Number(route.params.courseId)

    const posts = ref([])
    const loading = ref(true)
    const courseName = ref('')
    const activePost = ref(null)
    const newComment = ref('')
    const submittingComment = ref(false)

    const showCreateModal = ref(false)
    const createForm = ref({ title: '', content: '' })
    const createError = ref('')
    const creating = ref(false)

    const showDeletePostModal = ref(false)
    const deletingPost = ref(null)
    const deleting = ref(false)

    onMounted(async () => {
        await Promise.all([loadPosts(), loadCourseName()])
    })

    async function loadPosts() {
        loading.value = true
        try {
            const res = await discussionService.getPosts(courseId)
            posts.value = res.data
        } finally {
            loading.value = false
        }
    }

    async function loadCourseName() {
        try {
            const res = await courseService.getCourseDetail(courseId)
            courseName.value = res.data.title
        } catch { /* ignore */ }
    }

    async function openPost(post) {
        const res = await discussionService.getPost(courseId, post.id)
        activePost.value = res.data
    }

    function openCreateModal() {
        createForm.value = { title: '', content: '' }
        createError.value = ''
        showCreateModal.value = true
    }

    async function submitPost() {
        if (!createForm.value.title.trim() || !createForm.value.content.trim()) {
            createError.value = 'Title and content are required.'
            return
        }
        creating.value = true
        try {
            await discussionService.createPost(courseId, createForm.value)
            showCreateModal.value = false
            await loadPosts()
        } catch (err) {
            createError.value = err.response?.data?.message || 'Failed to create post.'
        } finally {
            creating.value = false
        }
    }

    async function submitComment() {
        if (!newComment.value.trim()) return
        submittingComment.value = true
        try {
            const res = await discussionService.addComment(courseId, activePost.value.id, {
                content: newComment.value.trim()
            })
            activePost.value.comments.push(res.data)
            newComment.value = ''
            // Update comment count in list
            const post = posts.value.find(p => p.id === activePost.value.id)
            if (post) post.commentCount++
        } catch (err) {
            toast.error(err.response?.data?.message || 'Failed to add comment.')
        } finally {
            submittingComment.value = false
        }
    }

    async function deleteComment(comment) {
        try {
            await discussionService.deleteComment(courseId, activePost.value.id, comment.id)
            activePost.value.comments = activePost.value.comments.filter(c => c.id !== comment.id)
            const post = posts.value.find(p => p.id === activePost.value.id)
            if (post) post.commentCount--
        } catch (err) {
            toast.error(err.response?.data?.message || 'Failed to delete comment.')
        }
    }

    function confirmDeletePost(post) {
        deletingPost.value = post
        showDeletePostModal.value = true
    }

    async function executeDeletePost() {
        deleting.value = true
        try {
            await discussionService.deletePost(courseId, deletingPost.value.id)
            showDeletePostModal.value = false
            await loadPosts()
        } finally {
            deleting.value = false
        }
    }

    function canDelete(post) {
        return post.userId === user?.id ||
            user?.role === 'Admin' ||
            user?.role === 'Instructor'
    }

    function canDeleteComment(comment) {
        return comment.userId === user?.id ||
            user?.role === 'Admin' ||
            user?.role === 'Instructor'
    }

    function goBack() {
        const role = user?.role
        if (role === 'Instructor') router.push('/instructor/dashboard')
        else router.push(`/student/courses/${courseId}`)
    }

    function formatDate(dateStr) {
        return new Date(dateStr).toLocaleDateString('en-US', {
            year: 'numeric', month: 'short', day: 'numeric', hour: '2-digit', minute: '2-digit'
        })
    }
</script>