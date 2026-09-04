<script setup>
import { ref } from 'vue'
import { useRoute } from 'vue-router'

const route = useRoute()
const bucketKey = route.params.bucketKey

const notice = ref('')
const isNoticeOpen = ref(false)

function onUploadModel() {
  console.log('Upload model requested for', bucketKey)
  notice.value = 'Uploading is not built yet.'
  isNoticeOpen.value = true
}
</script>

<template>
  <v-container class="py-8">
    <v-empty-state
      icon="mdi-cube-outline"
      title="No models yet"
      :text="`This folder is empty. Upload a model to ${bucketKey}.`"
    >
      <template #actions>
        <v-btn color="primary" variant="flat" prepend-icon="mdi-upload" @click="onUploadModel">
          Upload model
        </v-btn>
      </template>
    </v-empty-state>

    <v-snackbar v-model="isNoticeOpen" :text="notice" />
  </v-container>
</template>
