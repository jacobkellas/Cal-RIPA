<template>
  <v-dialog v-model="model" max-width="1000px">
    <v-card>
      <v-card-title>
        <span>{{ getTitle() }}</span>
      </v-card-title>

      <v-card-text>
        <v-container>
          <v-row>
            <v-col cols="12">
              <ripa-favorites-grid
                :items="favorites"
                :is-online-and-authenticated="isOnlineAndAuthenticated"
                :on-delete-favorite="onDeleteFavorite"
                :on-edit-favorite="onEditFavorite"
                :on-open-favorite="onOpenFavorite"
              ></ripa-favorites-grid>
            </v-col>
          </v-row>
        </v-container>
      </v-card-text>

      <v-card-actions>
        <v-spacer></v-spacer>
        <v-btn color="blue darken-1" text @click="handleClose"> Cancel </v-btn>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script>
import RipaFavoritesGrid from '@/components/molecules/RipaFavoritesGrid'

export default {
  name: 'ripa-favorites-dialog',

  components: {
    RipaFavoritesGrid,
  },

  data() {
    return {
      viewModel: this.showDialog,
    }
  },

  computed: {
    model: {
      get() {
        return this.viewModel
      },
      set(newValue) {
        if (!newValue) {
          if (this.onClose) {
            this.onClose()
          }
        }
        this.viewModel = newValue
      },
    },
  },

  methods: {
    handleClose() {
      if (this.onClose) {
        this.onClose()
      }
    },
    getTitle() {
      return 'Favorite ' + this.title
    },
  },

  watch: {
    showDialog(newValue) {
      this.viewModel = newValue
    },
  },

  props: {
    title: {
      type: String,
      default: '',
    },
    favorites: {
      type: Array,
      default: () => [],
    },
    isOnlineAndAuthenticated: {
      type: Boolean,
      default: false,
    },
    showDialog: {
      type: Boolean,
      default: false,
    },
    onClose: {
      type: Function,
      default: () => {},
    },
    onDeleteFavorite: {
      type: Function,
      default: () => {},
    },
    onEditFavorite: {
      type: Function,
      default: () => {},
    },
    onOpenFavorite: {
      type: Function,
      default: () => {},
    },
    onSaveFavorite: {
      type: Function,
      default: () => {},
    },
  },
}
</script>
