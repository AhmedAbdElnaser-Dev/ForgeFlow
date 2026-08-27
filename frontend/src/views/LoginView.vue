<script setup>
function onLoginWithAutodesk() {
  console.log('Autodesk login triggered')
}
</script>

<template>
  <v-container class="fill-height d-flex align-center justify-center">
    <v-card class="pa-6 text-center" max-width="420" width="100%" elevation="2">
      <h1 class="text-h6 mb-1">Sign in to ForgeFlow</h1>
      <p class="text-body-2 text-medium-emphasis mb-6">
        Use your Autodesk account to continue.
      </p>

      <v-btn
        color="primary"
        size="large"
        block
        prepend-icon="mdi-login-variant"
        @click="onLoginWithAutodesk"
      >
        Login with Autodesk
      </v-btn>
    </v-card>
  </v-container>
</template>
