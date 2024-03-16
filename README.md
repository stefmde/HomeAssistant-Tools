# HomeAssistant-Tools

Some tools for Home Assistant for Developers

## Delete Entities
This App can delete Entities that has no `unique_id` set. This can happen if you create an entity/state over the [ReST API](https://developers.home-assistant.io/docs/api/rest/) POST `/api/states/<entity_id>` of Home Assistant. You can't edit or delete those over the Web-UI.
**Warning:** Be sure to have a current Backup before using this App!
