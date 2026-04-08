<template>
    <RouterView v-slot="{ Component }">
        <Transition name="fade" mode="out-in">
            <component :is="Component" :key="$route.path" />
        </Transition>
    </RouterView>
    <ToastContainer />
</template>

<script setup>
    import { RouterView } from 'vue-router'
    import ToastContainer from './components/ToastContainer.vue'
</script>

<style>
    .fade-enter-active,
    .fade-leave-active {
        transition: opacity 0.15s ease;
    }

    .fade-enter-from,
    .fade-leave-to {
        opacity: 0;
    }
</style>