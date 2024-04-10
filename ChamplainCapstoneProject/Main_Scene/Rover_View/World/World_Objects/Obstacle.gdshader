shader_type canvas_item;

void vertex() {
	// Called for every vertex the material is visible on.
}

void fragment() {
	// Called for every pixel the material is visible on.
	vec3 line_col = vec3(0, 0.686, 0);
	vec3 bg_col = vec3(0.102);
	float boarder_size = 2.0;		
	
	float draw_line = step(float(int(FRAGCOORD.x + FRAGCOORD.y) % 16), 2.0);	
	
	vec3 final_col = (draw_line * line_col) + ((1.0-draw_line) * bg_col);
	COLOR = vec4(final_col,1);
}

//void light() {
	// Called for every pixel for every light affecting the CanvasItem.
	// Uncomment to replace the default light processing function with this one.
//}
