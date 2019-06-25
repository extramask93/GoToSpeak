export interface User {
    id: number;
    userName: string;
    lastActive: Date;
    photoUrl: string;
    isNewMessage: boolean;
    roles?: string[];
}
