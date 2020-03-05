export interface SwaggerSpecProperty {
  field: string;
  type: SwaggerSpecPropertyType;
}

export enum SwaggerSpecPropertyType {
  String = 'string',
  Integer = 'integer',
  Object = 'object',
  Boolean = 'boolean',
  Number = 'number',
  Array = 'array',
}
