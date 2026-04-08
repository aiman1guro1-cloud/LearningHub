<template>
    <div class="min-h-screen bg-gray-50">

        <!-- Navbar -->
        <nav class="bg-white border-b border-gray-200 px-6 py-4 flex items-center justify-between">
            <div class="flex items-center gap-3">
                <div class="w-8 h-8 bg-primary-600 rounded-lg flex items-center justify-center">
                    <span class="text-white font-bold text-sm">L</span>
                </div>
                <span class="font-semibold text-gray-800">LearningHub</span>
                <span class="text-gray-300">|</span>
                <span class="text-sm text-gray-500">Instructor</span>
            </div>
            <div class="flex items-center gap-4">
                <RouterLink :to="`/profile`"
                            class="text-sm text-gray-600 hover:text-primary-600 transition-colors cursor-pointer">
                    {{ user?.fullName }}
                </RouterLink>
                <button @click="logout"
                        class="text-sm text-red-500 hover:text-red-700 font-medium">
                    Logout
                </button>
            </div>
        </nav>

        <div class="max-w-5xl mx-auto px-6 py-8">

            <!-- Header -->
            <div class="flex items-center justify-between mb-6">
                <div>
                    <h1 class="text-2xl font-bold text-gray-800">My Courses</h1>
                    <p class="text-gray-500 text-sm mt-1">Create and manage your course content</p>
                </div>
                <button @click="openCreateModal"
                        class="bg-primary-600 hover:bg-primary-700 text-white px-4 py-2 rounded-lg text-sm font-medium transition-colors">
                    + New Course
                </button>
            </div>

            <!-- Loading Skeletons -->
            <div v-if="loading" class="grid gap-4">
                <CardSkeleton v-for="n in 3" :key="n" />
            </div>

            <!-- Empty -->
            <div v-else-if="courses.length === 0" class="text-center py-16">
                <div class="text-5xl mb-4">📚</div>
                <p class="text-gray-500">No courses yet. Create your first course!</p>
            </div>

            <!-- Course Grid -->
            <div v-else class="grid gap-4">
                <div v-for="course in courses" :key="course.id"
                     class="bg-white rounded-xl border border-gray-200 p-5 flex items-center justify-between hover:shadow-sm transition-shadow">
                    <div class="flex-1">
                        <div class="flex items-center gap-2 mb-1">
                            <h3 class="font-semibold text-gray-800">{{ course.title }}</h3>
                            <span :class="course.isPublished
                ? 'bg-green-100 text-green-700'
                : 'bg-yellow-100 text-yellow-700'"
                                  class="text-xs px-2 py-0.5 rounded-full font-medium">
                                {{ course.isPublished ? 'Published' : 'Draft' }}
                            </span>
                        </div>
                        <p class="text-sm text-gray-500 line-clamp-1">{{ course.description }}</p>
                        <div class="flex gap-4 mt-2 text-xs text-gray-400">
                            <span>{{ course.moduleCount }} modules</span>
                            <span>{{ course.enrollmentCount }} students</span>
                        </div>
                    </div>
                    <div class="flex gap-2 ml-4">
                        <button @click="goToContent(course.id)"
                                class="text-sm bg-primary-50 text-primary-700 hover:bg-primary-100 px-3 py-1.5 rounded-lg font-medium transition-colors">
                            Manage
                        </button>
                        <!-- NEW Progress button -->
                        <button @click="goToProgress(course.id)"
                                class="text-sm bg-green-50 text-green-700 hover:bg-green-100 px-3 py-1.5 rounded-lg font-medium transition-colors">
                            Progress
                        </button>
                        <button @click="openEditModal(course)"
                                class="text-sm bg-gray-100 text-gray-600 hover:bg-gray-200 px-3 py-1.5 rounded-lg font-medium transition-colors">
                            Edit
                        </button>
                        <button @click="confirmDelete(course)"
                                class="text-sm bg-red-50 text-red-500 hover:bg-red-100 px-3 py-1.5 rounded-lg font-medium transition-colors">
                            Delete
                        </button>
                    </div>
                </div>
            </div>
        </div>

        <!-- Create / Edit Modal -->
        <div v-if="showModal"
             class="fixed inset-0 bg-black bg-opacity-40 flex items-center justify-center z-50 p-4">
            <div class="bg-white rounded-2xl shadow-xl w-full max-w-md p-6">
                <h2 class="text-lg font-bold text-gray-800 mb-4">
                    {{ editingCourse ? 'Edit Course' : 'New Course' }}
                </h2>

                <div v-if="modalError" class="bg-red-50 text-red-600 text-sm rounded-lg px-4 py-2 mb-4">
                    {{ modalError }}
                </div>

                <div class="space-y-4">
                    <div>
                        <label class="block text-sm font-medium text-gray-700 mb-1">Title</label>
                        <input v-model="form.title" type="text" placeholder="Course title"
                               class="w-full border border-gray-300 rounded-lg px-4 py-2.5 text-sm focus:outline-none focus:ring-2 focus:ring-primary-500" />
                    </div>
                    <div>
                        <label class="block text-sm font-medium text-gray-700 mb-1">Description</label>
                        <textarea v-model="form.description" rows="3" placeholder="What will students learn?"
                                  class="w-full border border-gray-300 rounded-lg px-4 py-2.5 text-sm focus:outline-none focus:ring-2 focus:ring-primary-500 resize-none" />
                    </div>
                    <div v-if="editingCourse" class="flex items-center gap-2">
                        <input v-model="form.isPublished" type="checkbox" id="published" class="w-4 h-4 text-primary-600" />
                        <label for="published" class="text-sm text-gray-700">Published (visible to students)</label>
                    </div>
                </div>

                <div class="flex justify-end gap-3 mt-6">
                    <button @click="closeModal"
                            class="px-4 py-2 text-sm text-gray-600 hover:text-gray-800 font-medium">
                        Cancel
                    </button>
                    <button @click="submitCourse" :disabled="saving"
                            class="px-4 py-2 bg-primary-600 hover:bg-primary-700 text-white text-sm font-medium rounded-lg disabled:opacity-60 transition-colors">
                        {{ saving ? 'Saving...' : (editingCourse ? 'Save Changes' : 'Create Course') }}
                    </button>
                </div>
            </div>
        </div>

        <!-- Delete Confirm Modal -->
        <div v-if="showDeleteModal"
             class="fixed inset-0 bg-black bg-opacity-40 flex items-center justify-center z-50 p-4">
            <div class="bg-white rounded-2xl shadow-xl w-full max-w-sm p-6 text-center">
                <div class="text-4xl mb-3">🗑️</div>
                <h2 class="text-lg font-bold text-gray-800 mb-2">Delete Course?</h2>
                <p class="text-sm text-gray-500 mb-6">
                    "<span class="font-medium">{{ deletingCourse?.title }}</span>" and all its content will be permanently deleted.
                </p>
                <div class="flex gap-3 justify-center">
                    <button @click="showDeleteModal = false"
                            class="px-4 py-2 text-sm text-gray-600 border border-gray-300 rounded-lg hover:bg-gray-50">
                        Cancel
                    </button>
                    <button @click="deleteCourse" :disabled="saving"
                            class="px-4 py-2 bg-red-500 hover:bg-red-600 text-white text-sm font-medium rounded-lg disabled:opacity-60">
                        {{ saving ? 'Deleting...' : 'Delete' }}
                    </button>
                </div>
            </div>
        </div>

    </div>
</template>

<script setup>
    import { ref, onMounted } from 'vue'
    import { useRouter } from 'vue-router'
    import { useAuthStore } from '../../stores/authStore'
    import { courseService } from '../../services/courseService'
    import CardSkeleton from '../../components/CardSkeleton.vue'

    const router = useRouter()
    const authStore = useAuthStore()
    const user = authStore.user

    const courses = ref([])
    const loading = ref(true)
    const saving = ref(false)
    const showModal = ref(false)
    const showDeleteModal = ref(false)
    const editingCourse = ref(null)
    const deletingCourse = ref(null)
    const modalError = ref('')

    const form = ref({ title: '', description: '', isPublished: false })

    onMounted(loadCourses)

    async function loadCourses() {
        loading.value = true
        try {
            const res = await courseService.getMyCourses()
            courses.value = res.data
        } finally {
            loading.value = false
        }
    }

    function openCreateModal() {
        editingCourse.value = null
        form.value = { title: '', description: '', isPublished: false }
        modalError.value = ''
        showModal.value = true
    }

    function openEditModal(course) {
        editingCourse.value = course
        form.value = { title: course.title, description: course.description, isPublished: course.isPublished }
        modalError.value = ''
        showModal.value = true
    }

    function closeModal() { showModal.value = false }

    async function submitCourse() {
        if (!form.value.title.trim()) {
            modalError.value = 'Title is required.'
            return
        }
        saving.value = true
        modalError.value = ''
        try {
            if (editingCourse.value) {
                await courseService.updateCourse(editingCourse.value.id, form.value)
            } else {
                await courseService.createCourse(form.value)
            }
            showModal.value = false
            await loadCourses()
        } catch (err) {
            modalError.value = err.response?.data?.message || 'Something went wrong.'
        } finally {
            saving.value = false
        }
    }

    function confirmDelete(course) {
        deletingCourse.value = course
        showDeleteModal.value = true
    }

    async function deleteCourse() {
        saving.value = true
        try {
            await courseService.deleteCourse(deletingCourse.value.id)
            showDeleteModal.value = false
            await loadCourses()
        } finally {
            saving.value = false
        }
    }

    function goToContent(courseId) {
        router.push(`/instructor/courses/${courseId}/content`)
    }

    // NEW: Navigate to course progress view
    function goToProgress(courseId) {
        router.push(`/instructor/courses/${courseId}/progress`)
    }

    function logout() {
        authStore.logout()
        router.push('/login')
    }
</script>