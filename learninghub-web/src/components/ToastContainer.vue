<template>
    <div class="fixed top-4 right-4 z-[9999] flex flex-col gap-2 pointer-events-none">
        <TransitionGroup name="toast">
            <div v-for="toast in toastStore.toasts"
                 :key="toast.id"
                 class="flex items-start gap-3 px-4 py-3 rounded-xl shadow-lg pointer-events-auto
               max-w-sm w-full border"
                 :class="{
          'bg-white border-green-200':  toast.type === 'success',
          'bg-white border-red-200':    toast.type === 'error',
          'bg-white border-blue-200':   toast.type === 'info',
          'bg-white border-amber-200':  toast.type === 'warning'
        }">
                <!-- Icon -->
                <span class="text-base shrink-0 mt-0.5">
                    <span v-if="toast.type === 'success'">✅</span>
                    <span v-else-if="toast.type === 'error'">❌</span>
                    <span v-else-if="toast.type === 'info'">ℹ️</span>
                    <span v-else>⚠️</span>
                </span>
                <!-- Message -->
                <p class="text-sm text-gray-700 flex-1">{{ toast.message }}</p>
                <!-- Close -->
                <button @click="toastStore.remove(toast.id)"
                        class="text-gray-300 hover:text-gray-500 shrink-0 text-lg leading-none">
                    ✕
                </button>
            </div>
        </TransitionGroup>
    </div>
</template>

<script setup>
import { useToastStore } from '../stores/toastStore'
const toastStore = useToastStore()
</script>

<style scoped>
    .toast-enter-active {
        transition: all 0.3s ease;
    }

    .toast-leave-active {
        transition: all 0.25s ease;
    }

    .toast-enter-from {
        opacity: 0;
        transform: translateX(100%);
    }

    .toast-leave-to {
        opacity: 0;
        transform: translateX(100%);
    }
</style>