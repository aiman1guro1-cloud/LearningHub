<template>
    <div class="min-h-screen bg-gray-50">

        <!-- Navbar -->
        <nav class="bg-white border-b border-gray-200 px-6 py-4 flex items-center justify-between">
            <div class="flex items-center gap-3">
                <div class="w-8 h-8 bg-primary-600 rounded-lg flex items-center justify-center">
                    <span class="text-white font-bold text-sm">L</span>
                </div>
                <span class="font-semibold text-gray-800">LearningHub</span>
            </div>
            <div class="flex items-center gap-4">
                <!-- Nav tabs -->
                <button @click="activeTab = 'my'"
                        :class="activeTab === 'my'
            ? 'text-primary-600 font-semibold border-b-2 border-primary-600'
            : 'text-gray-500 hover:text-gray-700'"
                        class="text-sm pb-0.5 transition-colors">
                    My Courses
                </button>
                <button @click="activeTab = 'browse'"
                        :class="activeTab === 'browse'
            ? 'text-primary-600 font-semibold border-b-2 border-primary-600'
            : 'text-gray-500 hover:text-gray-700'"
                        class="text-sm pb-0.5 transition-colors">
                    Browse Courses
                </button>
                <span class="text-gray-300">|</span>
                <RouterLink :to="`/profile`"
                            class="text-sm text-gray-600 hover:text-primary-600 transition-colors cursor-pointer">
                    {{ user?.fullName }}
                </RouterLink>
                <button @click="logout" class="text-sm text-red-500 hover:text-red-700 font-medium">
                    Logout
                </button>
            </div>
        </nav>

        <div class="max-w-5xl mx-auto px-6 py-8">

            <!-- ── My Courses Tab ─────────────────────────────── -->
            <div v-if="activeTab === 'my'">
                <!-- Header with stats (updated) -->
                <div class="mb-6">
                    <h1 class="text-2xl font-bold text-gray-800">My Courses</h1>
                    <p class="text-gray-500 text-sm mt-1">Continue where you left off</p>

                    <!-- Stats row -->
                    <div v-if="enrollments.length > 0" class="flex gap-4 mt-4">
                        <div class="bg-white border border-gray-200 rounded-xl px-4 py-3 text-center min-w-24">
                            <p class="text-2xl font-bold text-primary-600">{{ enrollments.length }}</p>
                            <p class="text-xs text-gray-500 mt-0.5">Enrolled</p>
                        </div>
                        <div class="bg-white border border-gray-200 rounded-xl px-4 py-3 text-center min-w-24">
                            <p class="text-2xl font-bold text-green-600">{{ completedCourses }}</p>
                            <p class="text-xs text-gray-500 mt-0.5">Completed</p>
                        </div>
                        <div class="bg-white border border-gray-200 rounded-xl px-4 py-3 text-center min-w-24">
                            <p class="text-2xl font-bold text-amber-500">{{ averageProgress }}%</p>
                            <p class="text-xs text-gray-500 mt-0.5">Avg. Progress</p>
                        </div>
                    </div>
                </div>

                <!-- Loading Skeletons -->
                <div v-if="loadingEnrollments" class="grid gap-4">
                    <CardSkeleton v-for="n in 3" :key="n" />
                </div>

                <div v-else-if="enrollments.length === 0" class="text-center py-16">
                    <div class="text-5xl mb-4">📖</div>
                    <p class="text-gray-500 mb-4">You haven't enrolled in any courses yet.</p>
                    <button @click="activeTab = 'browse'"
                            class="bg-primary-600 text-white px-4 py-2 rounded-lg text-sm font-medium hover:bg-primary-700">
                        Browse Courses
                    </button>
                </div>

                <div v-else class="grid gap-4">
                    <div v-for="enrollment in enrollments" :key="enrollment.id"
                         class="bg-white rounded-xl border border-gray-200 p-5 hover:shadow-sm transition-shadow">
                        <div class="flex items-start justify-between">
                            <div class="flex-1">
                                <h3 class="font-semibold text-gray-800 mb-1">{{ enrollment.courseTitle }}</h3>
                                <p class="text-xs text-gray-400 mb-3">
                                    Enrolled {{ formatDate(enrollment.enrolledAt) }}
                                </p>

                                <!-- Progress Bar -->
                                <div class="flex items-center gap-3">
                                    <div class="flex-1 bg-gray-100 rounded-full h-2">
                                        <div class="bg-primary-500 h-2 rounded-full transition-all"
                                             :style="{ width: enrollment.progressPercent + '%' }">
                                        </div>
                                    </div>
                                    <span class="text-xs text-gray-500 whitespace-nowrap">
                                        {{ enrollment.completedMaterials }}/{{ enrollment.totalMaterials }}
                                        ({{ enrollment.progressPercent }}%)
                                    </span>
                                </div>
                            </div>

                            <div class="flex gap-2 ml-4">
                                <button @click="goToCourse(enrollment.courseId)"
                                        class="text-sm bg-primary-50 text-primary-700 hover:bg-primary-100 px-3 py-1.5 rounded-lg font-medium">
                                    Continue
                                </button>
                                <button @click="confirmUnenroll(enrollment)"
                                        class="text-sm bg-red-50 text-red-500 hover:bg-red-100 px-3 py-1.5 rounded-lg font-medium">
                                    Unenroll
                                </button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <!-- ── Browse Tab ─────────────────────────────────── -->
            <div v-if="activeTab === 'browse'">
                <div class="mb-6">
                    <h1 class="text-2xl font-bold text-gray-800">Browse Courses</h1>
                    <p class="text-gray-500 text-sm mt-1">Discover new courses to learn from</p>
                </div>

                <!-- Search -->
                <div class="mb-6">
                    <input v-model="searchQuery" type="text" placeholder="Search courses..."
                           class="w-full border border-gray-300 rounded-lg px-4 py-2.5 text-sm focus:outline-none focus:ring-2 focus:ring-primary-500" />
                </div>

                <div v-if="loadingPublished" class="text-center py-16 text-gray-400">
                    Loading courses...
                </div>

                <div v-else-if="filteredCourses.length === 0" class="text-center py-16">
                    <div class="text-5xl mb-4">🔍</div>
                    <p class="text-gray-500">No courses found.</p>
                </div>

                <div v-else class="grid gap-4 sm:grid-cols-2">
                    <div v-for="course in filteredCourses" :key="course.id"
                         class="bg-white rounded-xl border border-gray-200 p-5 flex flex-col hover:shadow-sm transition-shadow">
                        <div class="flex-1">
                            <h3 class="font-semibold text-gray-800 mb-1">{{ course.title }}</h3>
                            <p class="text-sm text-gray-500 line-clamp-2 mb-3">{{ course.description }}</p>
                            <div class="flex gap-3 text-xs text-gray-400 mb-4">
                                <span>👤 {{ course.instructorName }}</span>
                                <span>📦 {{ course.moduleCount }} modules</span>
                                <span>👥 {{ course.enrollmentCount }} enrolled</span>
                            </div>
                        </div>

                        <button v-if="!isEnrolledIn(course.id)"
                                @click="enroll(course)"
                                :disabled="enrollingId === course.id"
                                class="w-full bg-primary-600 hover:bg-primary-700 text-white py-2 rounded-lg text-sm font-medium transition-colors disabled:opacity-60">
                            {{ enrollingId === course.id ? 'Enrolling...' : 'Enroll Now' }}
                        </button>

                        <button v-else
                                @click="goToCourse(course.id)"
                                class="w-full bg-green-50 text-green-700 hover:bg-green-100 py-2 rounded-lg text-sm font-medium transition-colors">
                            ✓ Go to Course
                        </button>
                    </div>
                </div>
            </div>
        </div>

        <!-- Unenroll Confirm Modal -->
        <div v-if="showUnenrollModal"
             class="fixed inset-0 bg-black bg-opacity-40 flex items-center justify-center z-50 p-4">
            <div class="bg-white rounded-2xl shadow-xl w-full max-w-sm p-6 text-center">
                <div class="text-4xl mb-3">⚠️</div>
                <h2 class="text-lg font-bold text-gray-800 mb-2">Unenroll from course?</h2>
                <p class="text-sm text-gray-500 mb-6">
                    You will lose your progress in
                    "<span class="font-medium">{{ unenrollingItem?.courseTitle }}</span>".
                </p>
                <div class="flex gap-3 justify-center">
                    <button @click="showUnenrollModal = false"
                            class="px-4 py-2 text-sm border border-gray-300 rounded-lg hover:bg-gray-50">
                        Cancel
                    </button>
                    <button @click="executeUnenroll" :disabled="saving"
                            class="px-4 py-2 bg-red-500 text-white text-sm rounded-lg disabled:opacity-60 hover:bg-red-600">
                        {{ saving ? 'Unenrolling...' : 'Unenroll' }}
                    </button>
                </div>
            </div>
        </div>

    </div>
</template>

<script setup>
    import { ref, computed, onMounted } from 'vue'
    import { useRouter } from 'vue-router'
    import { useAuthStore } from '../../stores/authStore'
    import { useToastStore } from '../../stores/toastStore'
    import { enrollmentService } from '../../services/enrollmentService'
    import { courseService } from '../../services/courseService'
    import CardSkeleton from '../../components/CardSkeleton.vue'

    const router = useRouter()
    const authStore = useAuthStore()
    const toast = useToastStore()
    const user = authStore.user

    const activeTab = ref('my')
    const enrollments = ref([])
    const publishedCourses = ref([])
    const loadingEnrollments = ref(true)
    const loadingPublished = ref(true)
    const enrollingId = ref(null)
    const saving = ref(false)
    const searchQuery = ref('')

    // Unenroll modal
    const showUnenrollModal = ref(false)
    const unenrollingItem = ref(null)

    onMounted(async () => {
        await Promise.all([loadEnrollments(), loadPublishedCourses()])
    })

    async function loadEnrollments() {
        loadingEnrollments.value = true
        try {
            const res = await enrollmentService.getMyEnrollments()
            enrollments.value = res.data
        } finally {
            loadingEnrollments.value = false
        }
    }

    async function loadPublishedCourses() {
        loadingPublished.value = true
        try {
            const res = await courseService.getPublished()
            publishedCourses.value = res.data
        } finally {
            loadingPublished.value = false
        }
    }

    const filteredCourses = computed(() => {
        if (!searchQuery.value.trim()) return publishedCourses.value
        const q = searchQuery.value.toLowerCase()
        return publishedCourses.value.filter(c =>
            c.title.toLowerCase().includes(q) ||
            c.description.toLowerCase().includes(q) ||
            c.instructorName.toLowerCase().includes(q)
        )
    })

    // NEW: computed stats for the summary row
    const completedCourses = computed(() =>
        enrollments.value.filter(e => e.progressPercent === 100).length
    )

    const averageProgress = computed(() => {
        if (enrollments.value.length === 0) return 0
        const total = enrollments.value.reduce((sum, e) => sum + e.progressPercent, 0)
        return Math.round(total / enrollments.value.length)
    })

    function isEnrolledIn(courseId) {
        return enrollments.value.some(e => e.courseId === courseId)
    }

    async function enroll(course) {
        enrollingId.value = course.id
        try {
            await enrollmentService.enroll(course.id)
            await loadEnrollments()
            toast.success(`Enrolled in "${course.title}" successfully!`)
        } catch (err) {
            toast.error(err.response?.data?.message || 'Enrollment failed.')
        } finally {
            enrollingId.value = null
        }
    }

    function confirmUnenroll(enrollment) {
        unenrollingItem.value = enrollment
        showUnenrollModal.value = true
    }

    async function executeUnenroll() {
        saving.value = true
        try {
            await enrollmentService.unenroll(unenrollingItem.value.courseId)
            showUnenrollModal.value = false
            await loadEnrollments()
        } finally {
            saving.value = false
        }
    }

    function goToCourse(courseId) {
        router.push(`/student/courses/${courseId}`)
    }

    function formatDate(dateStr) {
        return new Date(dateStr).toLocaleDateString('en-US', {
            year: 'numeric', month: 'short', day: 'numeric'
        })
    }

    function logout() {
        authStore.logout()
        router.push('/login')
    }
</script>