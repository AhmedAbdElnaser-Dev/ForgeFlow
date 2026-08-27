<script setup>
import { computed } from 'vue'
import { RouterView, useRoute } from 'vue-router'

const route = useRoute()
const showAppBar = computed(() => !route.meta.public)
</script>

<template>
  <v-app>
    <v-app-bar v-if="showAppBar" color="primary" flat>
      <v-app-bar-title>ForgeFlow</v-app-bar-title>
    </v-app-bar>

    <v-main>
      <RouterView />
    </v-main>
  </v-app>
</template>
